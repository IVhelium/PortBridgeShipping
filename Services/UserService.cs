using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

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
            _filePath = Path.Combine(_folder, "users.json");
        }

        private record UserRecord(string Username, string Salt, string PasswordHash);

        private List<UserRecord> LoadUsers()
        {
            if (!File.Exists(_filePath)) return new List<UserRecord>();

            try
            {
                var json = File.ReadAllText(_filePath);
                var users = JsonSerializer.Deserialize<List<UserRecord>>(json);
                return users ?? new List<UserRecord>();
            }
            catch
            {
                return new List<UserRecord>();
            }
        }

        private void SaveUsers(List<UserRecord> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
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