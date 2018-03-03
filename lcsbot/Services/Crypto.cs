using System;
using System.IO;

namespace lcsbot.Services.Crypto
{
    class Crypto
    {
        private string path = "db.hs";

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public bool SaveHashToFile(string hash)
        {
            try
            {
                File.WriteAllText(path, hash);
                return true;
            }
            catch (Exception e)
            {
                Debugging.Log("SaveHashToFile", $"Error writing to file: {e.Message}");
                return false;
            }
        }

        public string ReadHashFromFile()
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Debugging.Log("SaveHashToFile", $"Error writing to file: {e.Message}");
                return null;
            }
        }
    }
}
