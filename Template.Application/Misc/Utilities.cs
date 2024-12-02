using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;
using bCrypt = BCrypt.Net.BCrypt;

namespace Template.Application.Misc
{
    public static class Utilities
    {
        private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static Random rnd = new Random();

        public static string GenerateOTP()
        {
            Random generator = new Random();
            String otp = generator.Next(0, 999999).ToString("D6");
            return otp;
        }
        public static string ReadHtml(string filepath)
        {
            string text = File.ReadAllText(filepath);
            return text;
        }
        public static StringBuilder ReadHtmlFile(string htmlFileNameWithPath)
        {
            StringBuilder htmlContent = new System.Text.StringBuilder();
            string line;
            try
            {
                using (System.IO.StreamReader htmlReader = new System.IO.StreamReader(htmlFileNameWithPath))
                {

                    while ((line = htmlReader.ReadLine()) != null)
                    {
                        htmlContent.Append(line);
                    }
                }
            }
            catch (Exception objError)
            {
                throw objError;
            }

            return htmlContent;
        }
  
        public static string HashOTP(string otp)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the OTP to a byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(otp));

                // Convert the byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Convert byte to hex string
                }
                return builder.ToString(); // Return the hashed OTP
            }
        }
        public static async Task<string> GetBAse64FromFilePath(string path)
        {
            Byte[] bytes = await File.ReadAllBytesAsync(path);
            String file = Convert.ToBase64String(bytes);
            return file;
        }
        public static class EncrytptionDecryptionHelper
        {
            public static string Hash(string plaintext)
            {
                var data = bCrypt.EnhancedHashPassword(plaintext, 12, hashType: HashType.SHA512);
                return data;
            }

            public static bool Verify(string plaintext, string hashed)
            {
                var verified = bCrypt.EnhancedVerify(plaintext, hashed, hashType: HashType.SHA512);
                return verified;
            }

        }
    }
}
