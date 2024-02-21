using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Skillup.XMLReadSearch
{
    public class AES_Cryptography
    {
        /*AesCryptoServiceProvider crypt_provider;
        public AES_Cryptography()
        {
            crypt_provider = new AesCryptoServiceProvider();

            crypt_provider.BlockSize = 128;
            crypt_provider.KeySize = 256;
            crypt_provider.GenerateIV();
            crypt_provider.GenerateKey();
            crypt_provider.Mode = CipherMode.CBC;
            crypt_provider.Padding = PaddingMode.PKCS7;
        }

        public String Encrypt(String clear_text)
        {
            ICryptoTransform transform = crypt_provider.CreateEncryptor();
            byte[] encrypted_bytes = transform.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes(clear_text),0,clear_text.Length);

            string str = Convert.ToBase64String(encrypted_bytes);

            return str;
        }

        public String Decrypt(String cipher_text)
        {
            ICryptoTransform transform = crypt_provider.CreateDecryptor();

            byte[] encr_bytes = Convert.FromBase64String(cipher_text);

            byte[] decrypyed_bytes = transform.TransformFinalBlock(encr_bytes, 0, encr_bytes.Length);
            string str = ASCIIEncoding.ASCII.GetString(decrypyed_bytes);
            return str;

            
        }*/

        public string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateKey(); // Generate a random key
                aesAlg.GenerateIV(); // Generate a random IV

                ICryptoTransform encryptor = aesAlg.CreateEncryptor();

                byte[] encryptedBytes = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);

                // Combine IV and encrypted data into a single byte array
                byte[] resultBytes = new byte[aesAlg.IV.Length + encryptedBytes.Length];
                Array.Copy(aesAlg.IV, 0, resultBytes, 0, aesAlg.IV.Length);
                Array.Copy(encryptedBytes, 0, resultBytes, aesAlg.IV.Length, encryptedBytes.Length);

                return Convert.ToBase64String(resultBytes);
            }
        }

        public string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // IV size is the same as block size

                // Extract IV from the beginning of the cipher bytes
                Array.Copy(cipherBytes, aesAlg.IV, aesAlg.IV.Length);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor();

                // Decrypt the data, starting after the IV
                byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, aesAlg.IV.Length, cipherBytes.Length - aesAlg.IV.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
