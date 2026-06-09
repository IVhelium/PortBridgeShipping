using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PortBridgeShipping.Services
{
    public class UserService
    {
        private readonly string _folder;
        private readonly string _filePath;

        public UserService()
        {
            _folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PortBridgeShipping");
            Directory.CreateDirectory(_folder);
            _filePath = Path.Combine(_folder, "users.dat"); // line-based storage using StreamReader/StreamWriter
        }

        private record UserRecord(string Username, string Salt, string PasswordHash);

        private List<UserRecord> LoadUsers()
        {
            var result = new List<UserRecord>();
            if (!File.Exists(_filePath)) return result;

            try
            {
                using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var sr = new StreamReader(fs, Encoding.UTF8);

                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // expected format: username|salt|hash
                    var parts = line.Split('|');
                    if (parts.Length != 3) continue;

                    var username = parts[0];
                    var salt = parts[1];
                    var hash = parts[2];

                    result.Add(new UserRecord(username, salt, hash));
                }
            }
            catch
            {
                // return empty list on error to keep service usable
                return new List<UserRecord>();
            }

            return result;
        }

        private void SaveUsers(List<UserRecord> users)
        {
            try
            {
                var tempPath = _filePath + ".tmp";
                using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    foreach (var u in users)
                    {
                        // Keep simple pipe-delimited, disallow pipe/newline in usernames at creation
                        sw.WriteLine($"{u.Username}|{u.Salt}|{u.PasswordHash}");
                    }
                }

                // Replace atomically when possible
                File.Copy(tempPath, _filePath, true);
                File.Delete(tempPath);
            }
            catch
            {
                // swallow IO exceptions — consider logging in a real app
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
            username = (username ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

            // Disallow delimiter/newline characters in username to preserve file format
            if (username.Contains('|') || username.Contains('\n') || username.Contains('\r')) return false;

            var users = LoadUsers();
            if (users.Exists(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase))) return false;

            var salt = GenerateSalt();
            var hash = ComputeHash(password, salt);
            users.Add(new UserRecord(username, salt, hash));
            SaveUsers(users);
            return true;
        }

        public bool ValidateCredentials(string username, string password)
        {
            username = (username ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

            var users = LoadUsers();
            var user = users.Find(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
            if (user == null) return false;

            var hash = ComputeHash(password, user.Salt);
            return string.Equals(hash, user.PasswordHash, StringComparison.OrdinalIgnoreCase);
        }

        public bool HasAnyUsers() => LoadUsers().Count > 0;
    }
}