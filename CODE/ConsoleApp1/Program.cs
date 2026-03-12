using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string password = "admin123";
        string salt = GenerateSalt();
        string hash = HashPassword(password, salt);

        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"Salt: {salt}");
        Console.WriteLine($"Hash: {hash}");
    }

    static string GenerateSalt()
    {
        byte[] saltBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    static string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] saltedPassword = Encoding.UTF8.GetBytes(salt + password);
            byte[] hashBytes = sha256.ComputeHash(saltedPassword);
            return Convert.ToBase64String(hashBytes);
        }
    }
}