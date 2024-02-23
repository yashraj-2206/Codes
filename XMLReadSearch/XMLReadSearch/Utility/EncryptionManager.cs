using System;
using System.Security.Cryptography;
using System.Text;


namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// Provides methods for encrypting and decrypting text using AES cryptography.
    /// </summary>
    public class EncryptionManager
    {
        /// <summary>
        /// Represents the provider for the AES cryptographic algorithm.
        /// </summary>
        private AesCryptoServiceProvider crypt_provider;

        /// <summary>
        /// Represents the encryption key used by the AES algorithm.
        /// </summary>
        private byte[] key = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// Represents the initialization vector (IV) used by the AES algorithm.
        /// </summary>
        private byte[] iv = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
       
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionManager"/> class with default encryption settings.
        /// </summary>
        public EncryptionManager()
        {
            crypt_provider = new AesCryptoServiceProvider();

            crypt_provider.BlockSize = 128;
            crypt_provider.KeySize = 256;
            crypt_provider.Mode = CipherMode.CBC;
            crypt_provider.Padding = PaddingMode.PKCS7;
        }

        /// <summary>
        /// Encrypts the specified clear text using AES encryption.
        /// </summary>
        /// <param name="clear_text"> The text to encrypt. </param>
        /// <returns>The encrypted text.</returns>
        public string Encrypt(string clear_text)
        {
            ICryptoTransform transform = crypt_provider.CreateEncryptor(key, iv);
            byte[] encrypted_bytes = transform.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes(clear_text), 0, clear_text.Length);

            return Convert.ToBase64String(encrypted_bytes);
        }

        /// <summary>
        /// Decrypts the specified cipher text using AES decryption.
        /// </summary>
        /// <param name="cipher_text">The text to decrypt.</param>
        /// <returns>The decrypted text.</returns>
        public string Decrypt(string cipher_text)
        {
            ICryptoTransform transform = crypt_provider.CreateDecryptor(key, iv);
            byte[] encr_bytes = Convert.FromBase64String(cipher_text);
            byte[] decrypted_bytes = transform.TransformFinalBlock(encr_bytes, 0, encr_bytes.Length);

            return ASCIIEncoding.ASCII.GetString(decrypted_bytes);
        }
    }

}
