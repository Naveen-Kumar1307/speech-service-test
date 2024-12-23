using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using NUnit.Framework;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Values
{
    [Serializable]
    public class TestSample
    {
        [XmlAttribute]
        public string Value { get; set; }
    }

    /// <summary>
    /// Verifies that each Conversion works as intended.
    /// </summary>
    [TestFixture]
    public class ConversionTestFixture
    {
        private Color sampleColor = Color.FromArgb(255, 5, 5, 5);
        private Color namedColor = Color.Blue;
        private Decimal sampleDecimal = new Decimal(5);
        private Point samplePoint = new Point(5, 5);
        private PointF samplePointF = new PointF(5.555f, 6.4444f);
        private Rectangle sampleRectangle = new Rectangle(5, 5, 5, 5);
        private RectangleF sampleRectangleF = new RectangleF(5.55f, 5.55f, 5.55f, 5.55f);
        private Size sampleSize = new Size(5, 5);
        private SizeF sampleSizeF = new SizeF(5.555f, 6.4444f);
        private DateTime sampleTime = 
            DateTime.ParseExact(
                DateTime.Now.ToString("s", CultureInfo.CurrentCulture), 
                "s", CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal);

        /// <summary>
        /// Verifies that converters work.
        /// </summary>
        [Test]
        public void ValueConverterConversions()
        {
            string nullText = null;
            ValueConverter converter = new ValueConverter();

            Assert.IsTrue(converter.ConvertTo<bool>(converter.ConvertFrom(true)));
            Assert.IsTrue(converter.ConvertTo<byte>(converter.ConvertFrom((byte)5)) == 5);
            Assert.IsTrue(converter.ConvertTo<char>(converter.ConvertFrom('5')) == '5');
            Assert.IsTrue(converter.ConvertTo<int>(converter.ConvertFrom(5)) == 5);
            Assert.IsTrue(converter.ConvertTo<long>(converter.ConvertFrom(5L)) == 5L);
            Assert.IsTrue(converter.ConvertTo<float>(converter.ConvertFrom(5.0f)) == 5);
            Assert.IsTrue(converter.ConvertTo<double>(converter.ConvertFrom(5.0)) == 5.0);
            Assert.IsTrue(converter.ConvertTo<string>(converter.ConvertFrom("5")) == "5");
            Assert.IsTrue(converter.ConvertFrom(nullText) == "");

            Assert.IsTrue(converter.ConvertTo<decimal>(converter.ConvertFrom(sampleDecimal)) == sampleDecimal);
            Assert.IsTrue(converter.ConvertTo<DateTime>(converter.ConvertFrom(sampleTime)) == sampleTime);
        }

        /// <summary>
        /// Verifies that bit operations work.
        /// </summary>
        [Test]
        public void BitOperations()
        {
            int[] bits = { 1, 3, 5 };
            int initialBits = 0;
            int testBits = initialBits.SetBits(bits);
            Assert.IsTrue(testBits.HasAllBits(bits));

            int revisedBits = testBits.ClearBit(1);
            Assert.IsTrue(revisedBits.HasAnyBits(bits));
            Assert.IsFalse(revisedBits.HasAllBits(bits));

            revisedBits = revisedBits.ChangeBit(1, true);
            Assert.IsTrue(revisedBits.HasAnyBits(bits));
            Assert.IsTrue(revisedBits.HasAllBits(bits));

            ImageFlags[] flags = testBits.ToArray<ImageFlags>();
            int flagBits = flags.ToBits();
            Assert.IsTrue(flagBits == testBits);

            int[] extremeBits = { 0, 31 };
            int[] strangeBits = { 1, 30 };
            int[] equivalents = { -2, 33 };
            int testStrange = initialBits.SetBits(strangeBits);
            int testSame = initialBits.SetBits(equivalents);
            Assert.IsTrue(testStrange == testSame);
        }

        /// <summary>
        /// Verifies that bit string operations work.
        /// </summary>
        [Test]
        public void BitStringOperations()
        {
            int[] bits = { 1, 3, 5 };
            string initialBits = string.Empty;
            string testBits = initialBits.SetBits(bits);
            Assert.IsTrue(testBits.HasAllBits(bits));

            string revisedBits = testBits.ClearBit(1);
            Assert.IsTrue(revisedBits.HasAnyBits(bits));
            Assert.IsFalse(revisedBits.HasAllBits(bits));

            revisedBits = revisedBits.ChangeBit(1, true);
            Assert.IsTrue(revisedBits.HasAnyBits(bits));
            Assert.IsTrue(revisedBits.HasAllBits(bits));

            ImageFlags[] flags = testBits.ToArray<ImageFlags>();
            string flagBits = flags.ToBitString();
            Assert.IsTrue(flagBits == testBits);
        }

        /// <summary>
        /// Verifies that it works as intended.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UnicodeConversionExceedsAvailableSpace()
        {
            string sampleText = "&#34234;&#32122;";
            string result = sampleText.WithXmlEntities(10);
        }

        /// <summary>
        /// Verifies that image conversions work.
        /// </summary>
        [Test]
        public void VerifyImageConversions()
        {
            FileInfo file = new FileInfo("SampleImage.jpg");
            Image sample = Image.FromFile(file.Name);
            int fileSize = (int)file.Length;

            // length of image buffer == original
            byte[] sampleBytes = ImageConversion.ToBytes(sample);
            Assert.IsTrue(sampleBytes.Length == fileSize);

            // length of perfect jpeg > image length
            byte[] perfectJpeg = ImageConversion.ToBytes(sample, 100);
            Assert.IsTrue(perfectJpeg.Length > fileSize);

            // default jpeg compression < perfect
            byte[] sampleJpeg = ImageConversion.ToBytes(sample, ImageFormat.Jpeg);
            Assert.IsTrue(sampleJpeg.Length < perfectJpeg.Length);

            // 40% compression < default
            byte[] squeezedJpeg = ImageConversion.ToBytes(sample, 60);
            Assert.IsTrue(squeezedJpeg.Length < sampleJpeg.Length);

            // reconversion of jpeg remembers quality (lengths are equal)
            Image anotherImage = ImageConversion.ToImage(sampleJpeg);
            byte[] anotherJpeg = ImageConversion.ToBytes(anotherImage, ImageFormat.Jpeg);
            Assert.IsTrue(anotherJpeg.Length < perfectJpeg.Length);

            // all subsequent conversions are conservative
            TestConversion(sampleBytes);
            TestConversion(perfectJpeg);
            TestConversion(sampleJpeg);
            TestConversion(squeezedJpeg);
        }

        /// <summary>
        /// Verifies that a conversion is conservative.
        /// </summary>
        /// <param name="imageBuffer">an image buffer</param>
        private void TestConversion(byte[] imageBuffer)
        {
            byte[] test = ImageConversion.ToBytes(ImageConversion.ToImage(imageBuffer));
            Assert.IsTrue(test.Length == imageBuffer.Length);
        }

        /// <summary>
        /// Verifies that a compatible copy works.
        /// </summary>
        [Test]
        public void VerifyCompatibleCopy()
        {
            Dictionary<string, string> sampleDic = new Dictionary<string, string>();
            sampleDic.Add("testKey", "testValue");

            Type testType = typeof(IDictionary<string, string>);
            object testDic = Conversion.ConvertTo(testType, sampleDic);
        }

        /// <summary>
        /// Verifies that array Conversions work.
        /// </summary>
        [Test]
        public void VerifyArrayConversions()
        {
            bool[] sampleBools = { true, false };
            Assert.IsTrue(Conversion.To<bool[]>()(Conversion.From<bool[]>()(sampleBools)).Length == 2);

            byte[] sampleBytes = { 1, 2, 3, 4, 5 };
            Assert.IsTrue(Conversion.To<byte[]>()(Conversion.From<byte[]>()(sampleBytes)).Length == 5);

            char[] sampleChars = { '1', '2', '3', '4', '5' };
            Assert.IsTrue(Conversion.To<char[]>()(Conversion.From<char[]>()(sampleChars)).Length == 5);

            int[] sampleInts = { 1, 2, 3, 4, 5 };
            Assert.IsTrue(Conversion.To<int[]>()(Conversion.From<int[]>()(sampleInts)).Length == 5);

            long[] sampleLongs = { 1L, 2L, 3L, 4L, 5L };
            Assert.IsTrue(Conversion.To<long[]>()(Conversion.From<long[]>()(sampleLongs)).Length == 5);

            float[] sampleFloats = { 1F, 2F, 3F, 4F, 5F };
            Assert.IsTrue(Conversion.To<float[]>()(Conversion.From<float[]>()(sampleFloats)).Length == 5);

            double[] sampleDoubles = { 1.0, 2.0, 3.0, 4.0, 5.0 };
            Assert.IsTrue(Conversion.To<double[]>()(Conversion.From<double[]>()(sampleDoubles)).Length == 5);

            String[] sampleTexts = { "1", "2", "3", "4", "5" };
            Assert.IsTrue(Conversion.To<String[]>()(Conversion.From<String[]>()(sampleTexts)).Length == 5);

            ImageFlags[] sampleEnums = { ImageFlags.Caching, ImageFlags.HasAlpha };
            Assert.IsTrue(Conversion.ToValues<ImageFlags>(Conversion.FromValues(sampleEnums)).Length == 2);
        }

        /// <summary>
        /// Verifies that each Conversion works as intended.
        /// </summary>
        [Test]
        public void VerifyEachConversion()
        {
            Assert.IsTrue(Conversion.To<bool>()(Conversion.From<bool>()(true)));
            Assert.IsTrue(Conversion.To<byte>()(Conversion.From<byte>()(5)) == 5);
            Assert.IsTrue(Conversion.To<char>()(Conversion.From<char>()('5')) == '5');
            Assert.IsTrue(Conversion.To<int>()(Conversion.From<int>()(5)) == 5);
            Assert.IsTrue(Conversion.To<long>()(Conversion.From<long>()(5L)) == 5L);
            Assert.IsTrue(Conversion.To<float>()(Conversion.From<float>()(5)) == 5);
            Assert.IsTrue(Conversion.To<double>()(Conversion.From<double>()(5.0)) == 5.0);
            Assert.IsTrue(Conversion.To<String>()(Conversion.From<String>()("5")) == "5");
            Assert.IsTrue(Conversion.To<String>()(Conversion.From<String>()(null)) == "");

            String testDecimal = Conversion.From<Decimal>()(sampleDecimal);
            Assert.IsTrue(Conversion.To<Decimal>()(testDecimal).Equals(sampleDecimal));

            String testTime = Conversion.From<DateTime>()(sampleTime);
            DateTime result = Conversion.To<DateTime>()(testTime);
            Assert.IsTrue(Conversion.To<DateTime>()(testTime) == sampleTime);

            String testColor = Conversion.From<Color>()(sampleColor);
            Assert.IsTrue(Conversion.To<Color>()(testColor).Equals(sampleColor));

            String textColor = Conversion.From<Color>()(namedColor);
            Assert.IsTrue(Conversion.To<Color>()(textColor).Equals(namedColor));

            String testPoint = Conversion.From<Point>()(samplePoint);
            Assert.IsTrue(Conversion.To<Point>()(testPoint).Equals(samplePoint));

            String testPointF = Conversion.From<PointF>()(samplePointF);
            Assert.IsTrue(Conversion.To<PointF>()(testPointF).Equals(samplePointF));

            String testSize = Conversion.From<Size>()(sampleSize);
            Assert.IsTrue(Conversion.To<Size>()(testSize).Equals(sampleSize));

            String testSizeF = Conversion.From<SizeF>()(sampleSizeF);
            Assert.IsTrue(Conversion.To<SizeF>()(testSizeF).Equals(sampleSizeF));

            String testRectangle = Conversion.From<Rectangle>()(sampleRectangle);
            Assert.IsTrue(Conversion.To<Rectangle>()(testRectangle).Equals(sampleRectangle));

            String testRectangleF = Conversion.From<RectangleF>()(sampleRectangleF);
            Assert.IsTrue(Conversion.To<RectangleF>()(testRectangleF).Equals(sampleRectangleF));

            String testEnum = Conversion.From<ImageFlags>()(ImageFlags.Caching);
            Assert.IsTrue(Conversion.To<ImageFlags>()(testEnum).Equals(ImageFlags.Caching));
        }

        /// <summary>
        /// Verifies that missing boolean value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingBooleanFails()
        {
            bool test = Conversion.To<bool>()(null);
        }

        /// <summary>
        /// Verifies that missing byte value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingByteFails()
        {
            byte test = Conversion.To<byte>()(null);
        }

        /// <summary>
        /// Verifies that missing character value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingCharacterFails()
        {
            char test = Conversion.To<char>()(null);
        }

        /// <summary>
        /// Verifies that missing integer value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingIntegerFails()
        {
            int test = Conversion.To<int>()(null);
        }

        /// <summary>
        /// Verifies that missing float value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingFloatFails()
        {
            float test = Conversion.To<float>()(null);
        }

        /// <summary>
        /// Verifies that missing double value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingDoubleFails()
        {
            double test = Conversion.To<double>()(null);
        }

        /// <summary>
        /// Verifies that missing decimal value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingDecimalFails()
        {
            Decimal test = Conversion.To<Decimal>()(null);
        }

        /// <summary>
        /// Verifies that missing time value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingTimeFails()
        {
            DateTime test = Conversion.To<DateTime>()(null);
        }

        /// <summary>
        /// Verifies that missing color value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingColorFails()
        {
            Color test = Conversion.To<Color>()(null);
        }

        /// <summary>
        /// Verifies that missing point value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingPointFails()
        {
            Point test = Conversion.To<Point>()(null);
        }

        /// <summary>
        /// Verifies that missing size value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingSizeFails()
        {
            Size test = Conversion.To<Size>()(null);
        }

        /// <summary>
        /// Verifies that missing rectangle value fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyMissingRectangleFails()
        {
            Rectangle test = Conversion.To<Rectangle>()(null);
        }

    } // ConversionTestFixture
}
