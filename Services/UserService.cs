using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PortBridgeShipping.Data;
using PortBridgeShipping.MVVM.Models;

namespace PortBridgeShipping.Services
{
    public class UserService
    {
        private readonly string _logPath;
        public string? LastError { get; private set; }

        public UserService()
        {
            // Ensure log path
            var logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PortBridgeShipping");
            try
            {
                Directory.CreateDirectory(logFolder);
            }
            catch { }

            _logPath = Path.Combine(logFolder, "user_service.log");

            // Ensure DB and schema exist
            try
            {
                using var db = new ApplicationDbContext();
                db.Database.EnsureCreated();

                // Make sure Users table exists even if migrations weren't applied
                try
                {
                    // Create table if not exists - SQLite dialect
                    db.Database.ExecuteSqlRaw(@"CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL,
                        Salt TEXT NOT NULL,
                        PasswordHash TEXT NOT NULL
                    );");

                    // Create unique index on username (case-sensitive by default) - create case-insensitive index using COLLATE NOCASE
                    db.Database.ExecuteSqlRaw(@"CREATE UNIQUE INDEX IF NOT EXISTS IX_Users_Username ON Users(Username COLLATE NOCASE);");
                }
                catch (Exception ex)
                {
                    try { File.AppendAllText(_logPath, $"[{DateTime.UtcNow}] SQL ensure table/index failed: {ex}\n"); } catch { }
                }
            }
            catch (Exception ex)
            {
                LastError = "Database initialization failed.";
                try { File.AppendAllText(_logPath, $"[{DateTime.UtcNow}] EnsureCreated failed: {ex}\n"); } catch { }
            }
        }

        private static string ComputeHash(string password, string salt)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        private static string GenerateSalt()
        {
            var bytes = new byte[16];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }

        public bool CreateUser(string username, string password)
        {
            LastError = null;

            username = (username ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(username))
            {
                LastError = "Username is required.";
                return false;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                LastError = "Password must be at least 8 characters.";
                return false;
            }

            // keep prior restriction from flat-file approach
            if (username.Contains('|') || username.Contains('\n') || username.Contains('\r'))
            {
                LastError = "Username contains invalid characters.";
                return false;
            }

            try
            {
                using var db = new ApplicationDbContext();

                // Ensure latest schema available (safe no-op if already applied)
                try { db.Database.Migrate(); } catch (Exception ex) { File.AppendAllText(_logPath, $"[{DateTime.UtcNow}] Migrate failed: {ex}\n"); }

                // Use a case-insensitive existence check via raw SQL/EF to avoid translation issues
                try
                {
                    var exists = db.Users.FromSqlRaw("SELECT * FROM Users WHERE Username = {0} COLLATE NOCASE", username).AsNoTracking().Any();
                    if (exists)
                    {
                        LastError = "Username already exists.";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    // Fallback to LINQ check in-memory
                    try
                    {
                        var existingUsernames = db.Users.AsNoTracking().Select(u => u.Username).ToList();
                        if (existingUsernames.Any(n => string.Equals(n, username, StringComparison.OrdinalIgnoreCase)))
                        {
                            LastError = "Username already exists.";
                            return false;
                        }
                    }
                    catch (Exception inner)
                    {
                        File.AppendAllText(_logPath, $"[{DateTime.UtcNow}] Existence check failed: {ex} | {inner}\n");
                        LastError = "Unable to verify username uniqueness.";
                        return false;
                    }
                }

                var salt = GenerateSalt();
                var hash = ComputeHash(password, salt);

                var user = new User
                {
                    Username = username,
                    Salt = salt,
                    PasswordHash = hash
                };

                db.Users.Add(user);
                db.SaveChanges();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                LastError = "Database update error during registration.";
                try { File.AppendAllText(_logPath, $"[{DateTime.UtcNow}] DbUpdateException in CreateUser: {dbEx}\n"); } catch { }
                return false;
            }
            catch (Exception ex)
            {
                LastError = "Unexpected error during registration.";
                try { File.AppendAllText(_logPath, $"[{DateTime.UtcNow}] Exception in CreateUser: {ex}\n"); } catch { }
                return false;
            }
        }

        public bool ValidateCredentials(string username, string password)
        {
            LastError = null;

            username = (username ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                LastError = "Username and password are required.";
                return false;
            }

            try
            {
                using var db = new ApplicationDbContext();
                var lower = username.ToLowerInvariant();
                var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Username.ToLower() == lower);
                if (user == null || string.IsNullOrEmpty(user.Salt) || string.IsNullOrEmpty(user.PasswordHash))
                {
                    LastError = "Invalid username or password.";
                    return false;
                }

                var computedHash = ComputeHash(password, user.Salt);

                try
                {
                    var a = Convert.FromHexString(computedHash);
                    var b = Convert.FromHexString(user.PasswordHash);
                    if (a.Length != b.Length)
                    {
                        LastError = "Invalid username or password.";
                        return false;
                    }

                    var equal = CryptographicOperations.FixedTimeEquals(a, b);
                    if (!equal) LastError = "Invalid username or password.";
                    return equal;
                }
                catch
                {
                    var eq = string.Equals(computedHash, user.PasswordHash, StringComparison.OrdinalIgnoreCase);
                    if (!eq) LastError = "Invalid username or password.";
                    return eq;
                }
            }
            catch (Exception ex)
            {
                LastError = "Unexpected error during authentication.";
                try { File.AppendAllText(_logPath, $"[{DateTime.UtcNow}] Exception in ValidateCredentials: {ex}\n"); } catch { }
                return false;
            }
        }

        public bool HasAnyUsers()
        {
            try
            {
                using var db = new ApplicationDbContext();
                return db.Users.Any();
            }
            catch (Exception ex)
            {
                LastError = "Error checking users.";
                try { File.AppendAllText(_logPath, $"[{DateTime.UtcNow}] Exception in HasAnyUsers: {ex}\n"); } catch { }
                return false;
            }
        }
    }
}