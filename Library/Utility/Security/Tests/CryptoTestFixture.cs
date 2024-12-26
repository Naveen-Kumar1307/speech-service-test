using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace GlobalEnglish.Utility.Security
{
    /// <summary>
    /// Verifies that crypto works as intended.
    /// </summary>
    [TestFixture]
    public class CryptoTestFixture
    {
        /// <summary>
        /// Verifies that initialized sample crypto works.
        /// </summary>
        [Test]
        public void VerifyInitialSample()
        {
            String test = "9876543210";
            Symmetric crypto = new Symmetric();
            String crypt = crypto.EncryptToHex(Symmetric.PackageText(test));
            String result = crypto.DecryptFromHex(crypt);
            Assert.IsTrue(Symmetric.UnpackText(result) == test);

            Symmetric dupe = Symmetric.With(crypto.Vector, crypto.Key);
            Assert.IsTrue(Symmetric.UnpackText(dupe.DecryptFromHex(crypt)) == test);
        }

        /// <summary>
        /// Verifies that standard sample crypto works.
        /// </summary>
        [Test]
        public void VerifyStandardSample()
        {
            String test = "9876543210";
            String vector = "0123456701234567";
            String key = "01234567012345670123456701234567";

            Symmetric crypto = Symmetric.With(vector, key);
            String crypt = crypto.EncryptToHex(Symmetric.PackageText(test));
            String result = crypto.DecryptFromHex(crypt);
            Assert.IsTrue(Symmetric.UnpackText(result) == test);
        }

        /// <summary>
        /// Verifies that hex conversions work.
        /// </summary>
        [Test]
        public void VerifyHexConversion()
        {
            int testValue = 987654321;
            byte[] test = BitConverter.GetBytes(testValue);
            byte[] result = Symmetric.GetBytes(Symmetric.GetHex(test));
            Assert.IsTrue(BitConverter.ToInt32(result, 0) == testValue);
        }

        /// <summary>
        /// Verifies that buffer packaging works.
        /// </summary>
        [Test]
        public void VerifyBufferPackaging()
        {
            int testValue = 987654321;
            byte[] test = BitConverter.GetBytes(testValue);
            byte[] package = Symmetric.Package(test);
            byte[] result = Symmetric.Unpack(package);
            Assert.IsTrue(result.Length == test.Length);
            Assert.IsTrue(BitConverter.ToInt32(result, 0) == testValue);
        }

        /// <summary>
        /// Verifies that improper header length fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void VerifyImproperHeaderFails()
        {
            String package = Symmetric.PackageText("112345", 1);
        }

        /// <summary>
        /// Verifies that insufficient header fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void VerifyInsufficientHeaderFails()
        {
            int count = 100;
            StringBuilder builder = new StringBuilder();
            while (count-- > 0) builder.Append("5");
            String package = Symmetric.PackageText(builder.ToString(), 2);
        }

        /// <summary>
        /// Verifies that decryption fails with improper data.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DecryptionFailsWithImproperData()
        {
            String test = "9876543210";
            String vector = "0123456701234567";
            String key = "01234567012345670123456701234567";

            Symmetric crypto = Symmetric.With(vector, key);
            String result = crypto.DecryptFromHex(test);
        }

        /// <summary>
        /// Verifies that decryption fails with missing hex data.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DecryptionFailsWithMissingHexData()
        {
            String vector = "0123456701234567";
            String key = "01234567012345670123456701234567";

            Symmetric crypto = Symmetric.With(vector, key);
            String result = crypto.DecryptFromHex(string.Empty);
        }

        /// <summary>
        /// Verifies that encryption fails with missing text data.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EncryptionFailsWithMissingTextData()
        {
            String vector = "0123456701234567";
            String key = "01234567012345670123456701234567";

            Symmetric crypto = Symmetric.With(vector, key);
            byte[] result = crypto.EncryptText(string.Empty);
        }

        /// <summary>
        /// Verifies that decryption fails with missing binary data.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DecryptionFailsWithMissingBinaryData()
        {
            byte[] test = { };
            String vector = "0123456701234567";
            String key = "01234567012345670123456701234567";

            Symmetric crypto = Symmetric.With(vector, key);
            byte[] result = crypto.Decrypt(test);
        }

        /// <summary>
        /// Verifies that encryption fails with missing binary data.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EncryptionFailsWithMissingBinaryData()
        {
            byte[] test = { };
            String vector = "0123456701234567";
            String key = "01234567012345670123456701234567";

            Symmetric crypto = Symmetric.With(vector, key);
            byte[] result = crypto.Encrypt(test);
        }

    } // CryptoTestFixture
}
