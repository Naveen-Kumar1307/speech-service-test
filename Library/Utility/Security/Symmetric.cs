using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Globalization;
using System.Collections.Generic;
using System.Security.Cryptography;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Security
{
    /// <summary>
    /// Performs symmetric cryptography with Rijndael.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows an initialization vector</item>
    /// <item>knows a cryptographic key</item>
    /// <item>encrypts supplied clear text values</item>
    /// <item>decrypts supplied encrypted values</item>
    /// </list>
    /// </remarks>
    public class Symmetric
    {
        private static readonly int KeyWidth = 32; // 256 bits
        private static readonly int KeySize = KeyWidth * 8;

        #region creating instances
        /// <summary>
        /// Returns a new Symmetric configured with its vector and key.
        /// </summary>
        /// <param name="vectorName">a configured vector property name</param>
        /// <param name="keyName">a configured key property name</param>
        /// <returns>a new Symmetric</returns>
        public static Symmetric WithConfigured(String vectorName, String keyName)
        {
            return Symmetric.With(
                        ConfiguredValue.Named(vectorName), 
                        ConfiguredValue.Named(keyName));
        }

        /// <summary>
        /// Returns a new Symmetric.
        /// </summary>
        /// <param name="vector">a required initialization vector</param>
        /// <param name="key">a required cryptographic key</param>
        /// <returns>a new Symmetric</returns>
        public static Symmetric With(String vector, String key)
        {
            Argument.Check("key", key);
            Argument.Check("vector", vector);
            return Symmetric.With(
                        Encoding.ASCII.GetBytes(vector), 
                        Encoding.ASCII.GetBytes(key));
        }

        /// <summary>
        /// Returns a new Symmetric.
        /// </summary>
        /// <param name="vector">a required initialization vector</param>
        /// <param name="key">a required cryptographic key</param>
        /// <returns>a new Symmetric</returns>
        public static Symmetric With(byte[] vector, byte[] key)
        {
            Argument.Check("key", key);
            Argument.Check("vector", vector);
            Argument.CheckLimit("key.Length", key.Length, Argument.EQUAL, KeyWidth);
            Argument.CheckLimit("vector.Length", vector.Length, Argument.EQUAL, KeyWidth / 2);
            Symmetric result = new Symmetric();
            result.Vector = vector;
            result.Key = key;
            return result;
        }

        /// <summary>
        /// Constructs a new Symmetric.
        /// </summary>
        public Symmetric()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this with a new random vector and a new random key.
        /// </summary>
        public void Initialize()
        {
            Rijndael initial = InitialRijndael;
            Vector = initial.IV;
            Key = initial.Key;
        }

        /// <summary>
        /// Returns an initial Rijndael.
        /// </summary>
        private Rijndael InitialRijndael
        {
            get
            {
                Rijndael result = new RijndaelManaged();
                result.KeySize = KeySize;
                result.GenerateIV();
                result.GenerateKey();
                return result;
            }
        }
        #endregion

        #region crypto operations
        /// <summary>
        /// Returns an encoded hashed sleeve value.
        /// </summary>
        /// <param name="sleeve">a sleeve value</param>
        /// <returns>an encoded hashed sleeve value</returns>
        public static String EncodeHash(String sleeve)
        {
            return Convert.ToBase64String(HashSleeve(sleeve));
        }

        /// <summary>
        /// Returns a hashed sleeve value.
        /// </summary>
        /// <param name="sleeve">a sleeve value</param>
        /// <returns>a hashed sleeve value</returns>
        public static byte[] HashSleeve(String sleeve)
        {
            Argument.Check("sleeve", sleeve);
            try
            {
                byte[] passBytes = new UTF8Encoding().GetBytes(sleeve);
                return new SHA384Managed().ComputeHash(passBytes);
            }
            catch (Exception ex)
            {                
                throw new ArgumentOutOfRangeException("Supplied sleeve value was invalid", ex);
            }
        }

        /// <summary>
        /// Creates and encrypts a new random password.
        /// </summary>
        /// <param name="passwordLength">a password length</param>
        /// <param name="headerLength">a header length</param>
        /// <returns>an encrypted new password</returns>
        public byte[] EncryptNewRandomPassword(int passwordLength, int headerLength)
        {
            String password = CreatePassword(passwordLength);
            return EncryptText(PackageText(password, headerLength));
        }

        /// <summary>
        /// Encrypts a given clear text phrase into hex.
        /// </summary>
        /// <param name="clearPhrase">a required clear text phrase</param>
        /// <returns>encrypted text as hex</returns>
        public String EncryptToHex(String clearPhrase)
        {
            Argument.Check("clearPhrase", clearPhrase);
            return GetHex(EncryptText(clearPhrase));
        }

        /// <summary>
        /// Encrypts a given clear text phrase.
        /// </summary>
        /// <param name="clearPhrase">a required clear text phrase</param>
        /// <returns>encrypted phrase</returns>
        public byte[] EncryptText(String clearPhrase)
        {
            Argument.Check("clearPhrase", clearPhrase);
            return Encrypt(Encoding.ASCII.GetBytes(clearPhrase));
        }

        /// <summary>
        /// Encrypts a given clear phrase.
        /// </summary>
        /// <param name="clearPhrase">a required clear phrase</param>
        /// <returns>encrypted phrase</returns>
        public byte[] Encrypt(byte[] clearPhrase)
        {
            Argument.Check("clearPhrase", clearPhrase);
            return Encrypt(clearPhrase, clearPhrase.Length);
        }

        /// <summary>
        /// Encrypts a given clear phrase.
        /// </summary>
        /// <param name="clearPhrase">a required clear phrase</param>
        /// <param name="phraseLength">a required phrase length</param>
        /// <returns>encrypted phrase</returns>
        public byte[] Encrypt(byte[] clearPhrase, int phraseLength)
        {
            Argument.Check("clearPhrase", clearPhrase);
            Argument.CheckLimit("phraseLength", phraseLength, Argument.MORE, 0);
            using (MemoryStream stream = new MemoryStream())
            using (CryptoStream crypt = CreateEncryptionStream(stream))
            {
                crypt.Write(clearPhrase, 0, phraseLength);
                crypt.FlushFinalBlock();
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Decrypts a given encrypted password to clear text.
        /// </summary>
        /// <param name="cryptBytes">a required crypt phrase</param>
        /// <param name="headerLength">a header length</param>
        /// <returns>a clear text password</returns>
        public String DecryptPassword(byte[] cryptBytes, int headerLength)
        {
            String clearText = DecryptText(cryptBytes);
            return UnpackText(clearText, headerLength);
        }

        /// <summary>
        /// Decrypts a given hex crypt phrase to clear text.
        /// </summary>
        /// <param name="hexPhrase">a required hex crypt phrase</param>
        /// <returns>decrypted clear text</returns>
        public String DecryptFromHex(String hexPhrase)
        {
            return DecryptText(GetBytes(hexPhrase));
        }

        /// <summary>
        /// Decrypts a given crypt phrase to text.
        /// </summary>
        /// <param name="cryptPhrase">a required crypt phrase</param>
        /// <returns>decrypted clear text</returns>
        public String DecryptText(byte[] cryptPhrase)
        {
            return Encoding.ASCII.GetString(Decrypt(cryptPhrase));
        }

        /// <summary>
        /// Decrypts a given crypt phrase.
        /// </summary>
        /// <param name="cryptPhrase">a required crypt phrase</param>
        /// <returns>decrypted clear phrase</returns>
        public byte[] Decrypt(byte[] cryptPhrase)
        {
            try
            {
                byte[] results = new byte[cryptPhrase.Length];
                using (MemoryStream stream = new MemoryStream(cryptPhrase))
                using (CryptoStream crypt = CreateDecryptionStream(stream))
                {
                    crypt.Read(results, 0, results.Length);
                    return results;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException(DecryptionError, ex);
            }
        }

        private static readonly string DecryptionError = 
            "Supplied data was not properly encrypted for decryption by this cryptographer";
        #endregion

        #region configuring crypto
        /// <summary>
        /// Returns a new encryption stream.
        /// </summary>
        /// <param name="stream">a stream</param>
        /// <returns>a new CryptoStream</returns>
        private CryptoStream CreateEncryptionStream(Stream stream)
        {
            return new CryptoStream(stream, Encryptor, CryptoStreamMode.Write);
        }

        /// <summary>
        /// Returns a new decryption stream.
        /// </summary>
        /// <param name="stream">a stream</param>
        /// <returns>a new CryptoStream</returns>
        private CryptoStream CreateDecryptionStream(Stream stream)
        {
            return new CryptoStream(stream, Decryptor, CryptoStreamMode.Read);
        }

        /// <summary>
        /// An encryptor.
        /// </summary>
        private ICryptoTransform Encryptor
        {
            get { return ConfiguredRijndael.CreateEncryptor(); }
        }

        /// <summary>
        /// A decryptor.
        /// </summary>
        private ICryptoTransform Decryptor
        {
            get { return ConfiguredRijndael.CreateDecryptor(); }
        }

        /// <summary>
        /// Returns a configured Rijndael.
        /// </summary>
        private Rijndael ConfiguredRijndael
        {
            get
            {
                Rijndael result = new RijndaelManaged();
                result.Padding = PaddingMode.PKCS7;
                result.Mode = CipherMode.CBC;
                result.IV = Vector;
                result.Key = Key;
                return result;
            }
        }
        #endregion

        #region accessing values
        /// <summary>
        /// An initialization vector.
        /// </summary>
        public byte[] Vector { get; private set; }

        /// <summary>
        /// A cryptographic key.
        /// </summary>
        public byte[] Key { get; private set; }
        #endregion

        #region packaging text
        private static readonly int StandardHeader = 4;

        /// <summary>
        /// Returns a header format.
        /// </summary>
        /// <param name="headerLength">a header length</param>
        /// <returns>a header format</returns>
        private static String GetHeaderFormat(int headerLength)
        {
            StringBuilder builder = new StringBuilder();
            while (headerLength-- > 0) builder.Append("0");
            return builder.ToString();
        }

        /// <summary>
        /// Packages a given text buffer with a leading length header.
        /// </summary>
        /// <param name="text">a required text buffer</param>
        /// <returns>a packaged text buffer</returns>
        public static String PackageText(String text)
        {
            return PackageText(text, StandardHeader);
        }

        /// <summary>
        /// Packages a given text buffer with a leading length header.
        /// </summary>
        /// <param name="text">a required text buffer</param>
        /// <param name="headerLength">a header length</param>
        /// <returns>a packaged text buffer</returns>
        public static String PackageText(String text, int headerLength)
        {
            Argument.Check("text", text);
            Argument.CheckLimit("headerLength", headerLength, Argument.MORE, 1);
            String dataLength = text.Length.ToString();
            Argument.CheckLimit("headerLength", dataLength.Length, Argument.LESS, headerLength + 1);
            String headerFormat = GetHeaderFormat(headerLength);
            StringBuilder builder = new StringBuilder();
            builder.Append(text.Length.ToString(headerFormat));
            builder.Append(text);
            return builder.ToString();
        }

        /// <summary>
        /// Unpacks text from a packaged text buffer.
        /// </summary>
        /// <param name="text">a required text buffer</param>
        /// <returns>unpacked text</returns>
        public static String UnpackText(String text)
        {
            return UnpackText(text, StandardHeader);
        }

        /// <summary>
        /// Unpacks text from a packaged text buffer.
        /// </summary>
        /// <param name="text">a required text buffer</param>
        /// <param name="headerLength">a header length</param>
        /// <returns>unpacked text</returns>
        public static String UnpackText(String text, int headerLength)
        {
            Argument.Check("text", text);
            if (headerLength == 0) return text;
            int dataLength = int.Parse(text.Substring(0, headerLength));
            return text.Substring(headerLength, dataLength);
        }
        #endregion

        #region packaging bytes
        /// <summary>
        /// Packages a given data buffer with a leading length header.
        /// </summary>
        /// <param name="buffer">a required data buffer</param>
        /// <returns>a packaged buffer</returns>
        public static byte[] Package(byte[] buffer)
        {
            Argument.Check("buffer", buffer);
            byte[] bufferLength = BitConverter.GetBytes(buffer.Length);
            byte[] result = new byte[buffer.Length + bufferLength.Length];
            bufferLength.CopyTo(result, 0);
            buffer.CopyTo(result, bufferLength.Length);
            return result;
        }

        /// <summary>
        /// Unpacks data from a packaged buffer.
        /// </summary>
        /// <param name="buffer">a required packaged buffer</param>
        /// <returns>unpacked bytes</returns>
        public static byte[] Unpack(byte[] buffer)
        {
            Argument.Check("buffer", buffer);
            byte[] bufferLength = BitConverter.GetBytes(buffer.Length);
            int dataLength = BitConverter.ToInt32(buffer, 0);
            byte[] result = new byte[dataLength];
            Array.Copy(buffer, bufferLength.Length, result, 0, dataLength);
            return result;
        }
        #endregion

        #region converting bytes
        private static readonly int ByteWidth = 2; // nibbles per byte

        /// <summary>
        /// Converts hex text to bytes.
        /// </summary>
        /// <param name="hexText">required hex text</param>
        /// <returns>resulting bytes</returns>
        public static byte[] GetBytes(String hexText)
        {
            Argument.Check("hexText", hexText);
            hexText = hexText.Trim();

            if ((hexText.Length % ByteWidth) != 0)
                hexText = "0" + hexText;

            char[] pair = new char[ByteWidth];
            int count = hexText.Length / ByteWidth;
            using (MemoryStream stream = new MemoryStream())
            using (StringReader reader = new StringReader(hexText))
            {
                while (count > 0)
                {
                    reader.Read(pair, 0, ByteWidth);
                    stream.WriteByte(byte.Parse(new String(pair), NumberStyles.HexNumber));
                    count--;
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Converts a binary buffer to hex text.
        /// </summary>
        /// <param name="buffer">binary buffer</param>
        /// <returns>resulting hex text</returns>
        public static String GetHex(byte[] buffer)
        {
            Argument.Check("buffer", buffer);
            StringBuilder builder = new StringBuilder();
            foreach (byte value in buffer)
            {
                builder.Append(value.ToString("X2"));
            }
            return builder.ToString();
        }
        #endregion

        #region creating passwords
        /// <summary>
        /// A source of randomness.
        /// </summary>
        private static RNGCryptoServiceProvider RandomSource = new RNGCryptoServiceProvider();

        /// <summary>
        /// The valid letters and digits.
        /// </summary>
        private static string ValidLetters = 
            "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// The available password letters indexable by a byte value (0-255).
        /// </summary>
        private static string PasswordLetters = 
            ValidLetters + ValidLetters + ValidLetters + ValidLetters + "01234567";

        /// <summary>
        /// Verifies that a password has: 
        /// 1) at least a given length, 
        /// 2) at least one digit, 
        /// 3) only letters and digits.
        /// </summary>
        /// <param name="password">a password</param>
        /// <param name="length">a password length</param>
        /// <returns>whether a given password qualifies</returns>
        public static bool PasswordQualifies(string password, int length)
        {
            if (password == null || password.Length == 0) return false;
            if (password.Length < length) return false;

            bool digitFound = false;
            for (int index = 0; index < length; index++)
            {
                int position = ValidLetters.IndexOf(password[index]);
                if (position < 0) return false;
                if (position < 10) digitFound = true;
            }
            return digitFound;
        }

        /// <summary>
        /// Returns a new random password of a given length.  
        /// </summary>
        /// <param name="length">a password length</param>
        /// <returns>a new random password</returns>
        /// <remarks>
        /// A helper to generate passwords if a person is not available. Often, special 
        /// characters cause problems in the situations where a password needs to be 
        /// generated, so they are not supported in this routine. With people generated 
        /// passwords though, special characters are fine.
        /// </remarks>
        public static string CreatePassword(int length)
        {
            string result = CreateRandomPassword(length);
            while (!PasswordQualifies(result, length))
                result = CreateRandomPassword(length);

            return result;
        }

        /// <summary>
        /// Creates a random password of a given length.
        /// </summary>
        /// <param name="length">a password length</param>
        /// <returns>a new random password</returns>
        private static string CreateRandomPassword(int length)
        {
            if (length <= 0) return string.Empty;
            byte[] randoms = new byte[length];
            RandomSource.GetBytes(randoms);

            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < length; index++)
            {
                builder.Append(PasswordLetters[randoms[index]]);
            }
            return builder.ToString();
        }
        #endregion

        /// <summary>
        /// A cryptographer source.
        /// </summary>
        public interface ISource
        {
            /// <summary>
            /// A cryptographer.
            /// </summary>
            Symmetric Cryptographer { get; }

        } // ISource

    } // Symmetric
}
