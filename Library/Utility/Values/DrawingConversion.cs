using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Globalization;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Values
{
    /// <summary>
    /// Converts a Point from and to a canonical string.
    /// </summary>
    internal class PointConversion : IntegerPairConversion
    {
        private static readonly Type ConversionType = typeof(Point);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<Point, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, Point>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a point to a canonical string.
        /// </summary>
        /// <param name="point">a point</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(Point point)
        {
            int[] distances = { point.X, point.Y };
            return Convert(distances);
        }

        /// <summary>
        /// Converts a canonical string to a point.
        /// </summary>
        /// <param name="pointValue">a canonical string</param>
        /// <returns>a point</returns>
        public Point ConvertToValue(String pointValue)
        {
            Argument.Check("pointValue", pointValue);
            int[] distances = Convert(pointValue);
            return new Point(distances[0], distances[1]);
        }

    } // PointConversion

    /// <summary>
    /// Converts a PointF from and to a canonical string.
    /// </summary>
    internal class PointFConversion : FloatPairConversion
    {
        private static readonly Type ConversionType = typeof(PointF);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<PointF, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, PointF>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a point to a canonical string.
        /// </summary>
        /// <param name="point">a point</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(PointF point)
        {
            float[] distances = { point.X, point.Y };
            return Convert(distances);
        }

        /// <summary>
        /// Converts a canonical string to a point.
        /// </summary>
        /// <param name="pointValue">a canonical string</param>
        /// <returns>a point</returns>
        public PointF ConvertToValue(String pointValue)
        {
            Argument.Check("pointValue", pointValue);
            float[] distances = Convert(pointValue);
            return new PointF(distances[0], distances[1]);
        }

    } // PointFConversion

    /// <summary>
    /// Converts a Size from and to a canonical string.
    /// </summary>
    internal class SizeConversion : IntegerPairConversion
    {
        private static readonly Type ConversionType = typeof(Size);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<Size, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, Size>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a size to a canonical string.
        /// </summary>
        /// <param name="size">a size</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(Size size)
        {
            int[] distances = { size.Width, size.Height };
            return Convert(distances);
        }

        /// <summary>
        /// Converts a canonical string to a size.
        /// </summary>
        /// <param name="sizeValue">a canonical string</param>
        /// <returns>a size</returns>
        public Size ConvertToValue(String sizeValue)
        {
            Argument.Check("sizeValue", sizeValue);
            int[] distances = Convert(sizeValue);
            return new Size(distances[0], distances[1]);
        }

    } // SizeConversion

    /// <summary>
    /// Converts a SizeF from and to a canonical string.
    /// </summary>
    internal class SizeFConversion : FloatPairConversion
    {
        private static readonly Type ConversionType = typeof(SizeF);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<SizeF, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, SizeF>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a size to a canonical string.
        /// </summary>
        /// <param name="size">a size</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(SizeF size)
        {
            float[] distances = { size.Width, size.Height };
            return Convert(distances);
        }

        /// <summary>
        /// Converts a canonical string to a size.
        /// </summary>
        /// <param name="sizeValue">a canonical string</param>
        /// <returns>a size</returns>
        public SizeF ConvertToValue(String sizeValue)
        {
            Argument.Check("sizeValue", sizeValue);
            float[] distances = Convert(sizeValue);
            return new SizeF(distances[0], distances[1]);
        }

    } // SizeFConversion

    /// <summary>
    /// Converts a Rectangle from and to a canonical string.
    /// </summary>
    internal class RectangleConversion : IntegerQuadConversion
    {
        private static readonly Type ConversionType = typeof(Rectangle);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<Rectangle, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, Rectangle>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a rectangle to a canonical string.
        /// </summary>
        /// <param name="rectangle">a rectangle</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(Rectangle rectangle)
        {
            int[] values = { rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom };
            return Convert(values);
        }

        /// <summary>
        /// Converts a canonical string to a rectangle.
        /// </summary>
        /// <param name="rectangleValue">a canonical string</param>
        /// <returns>a rectangle</returns>
        public Rectangle ConvertToValue(String rectangleValue)
        {
            Argument.Check("pointValue", rectangleValue);
            int[] values = Convert(rectangleValue);
            return Rectangle.FromLTRB(values[0], values[1], values[2], values[3]);
        }

    } // RectangleConversion

    /// <summary>
    /// Converts RectangleF from and to a canonical string.
    /// </summary>
    internal class RectangleFConversion : FloatQuadConversion
    {
        private static readonly Type ConversionType = typeof(RectangleF);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<RectangleF, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, RectangleF>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a rectangle to a canonical string.
        /// </summary>
        /// <param name="rectangle">a rectangle</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(RectangleF rectangle)
        {
            float[] values = { rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom };
            return Convert(values);
        }

        /// <summary>
        /// Converts a canonical string to a rectangle.
        /// </summary>
        /// <param name="rectangleValue">a canonical string</param>
        /// <returns>a rectangle</returns>
        public RectangleF ConvertToValue(String rectangleValue)
        {
            Argument.Check("rectangleValue", rectangleValue);
            float[] values = Convert(rectangleValue);
            return RectangleF.FromLTRB(values[0], values[1], values[2], values[3]);
        }

    } // RectangleFConversion

    /// <summary>
    /// Converts a Color from and to a canonical string.
    /// </summary>
    internal class ColorConversion : IntegerQuadConversion
    {
        private static readonly Type ConversionType = typeof(Color);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<Color, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, Color>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a color to a canonical string.
        /// </summary>
        /// <param name="color">a color</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(Color color)
        {
            int argb = color.ToArgb();
            String hex = argb.ToString("x");
            if (color.Name == hex)
            {
                hex = argb.ToString("X8");
                return "0x" + hex;
            }
            else
            {
                return color.Name;
            }
        }

        /// <summary>
        /// Converts a canonical string to a color.
        /// </summary>
        /// <param name="colorValue">a canonical string</param>
        /// <returns>a color</returns>
        public Color ConvertToValue(String colorValue)
        {
            Argument.Check("colorValue", colorValue);
            String candidate = colorValue.Trim();
            if (candidate.StartsWith("0x"))
            {
                String hex = candidate.Substring(2, candidate.Length - 2);
                int argb = int.Parse(hex, NumberStyles.HexNumber);
                return Color.FromArgb(argb);
            }
            else
            {
                return Color.FromName(colorValue);
            }
        }

    } // ColorConversion


    /// <summary>
    /// Converts between an image and its content (image buffer).
    /// <a href="../overviews/ConversionModels.html">Models</a>
    /// </summary>
    public class ImageConversion
    {
        /// <summary>
        /// Converts an image buffer to a bitmap.
        /// </summary>
        /// <param name="imageBuffer">a required image buffer</param>
        /// <returns>a bitmap</returns>
        public static Bitmap ToBitmap(byte[] imageBuffer)
        {
            return (Bitmap)ToImage(imageBuffer);
        }

        /// <summary>
        /// Converts an image buffer to an image.
        /// </summary>
        /// <param name="imageBuffer">a required image buffer</param>
        /// <returns>an image</returns>
        public static Image ToImage(byte[] imageBuffer)
        {
            Argument.Check("imageBuffer", imageBuffer);
            MemoryStream stream = new MemoryStream(imageBuffer);
            return Image.FromStream(stream);
        }

        /// <summary>
        /// Converts an image to an image buffer.
        /// </summary>
        /// <param name="image">a required image</param>
        /// <returns>an image buffer</returns>
        public static byte[] ToBytes(Image image)
        {
            return ToBytes(image, image.RawFormat);
        }

        /// <summary>
        /// Converts an image to a given format and quality.
        /// </summary>
        /// <param name="image">a required image</param>
        /// <param name="format">a required image format</param>
        /// <param name="quality">a JPEG image quality</param>
        /// <returns>an image buffer</returns>
        /// <remarks>Note that if the supplied format is not JPEG, the quality is ignored.
        /// </remarks>
        public static byte[] ToBytes(Image image, ImageFormat format, int quality)
        {
            return (format.Guid == ImageFormat.Jpeg.Guid ?
                    ToBytes(image, quality) : 
                    ToBytes(image, format));
        }

        /// <summary>
        /// Converts an image to an image buffer.
        /// </summary>
        /// <param name="image">a required image</param>
        /// <param name="format">a required image format</param>
        /// <returns>an image buffer</returns>
        public static byte[] ToBytes(Image image, ImageFormat format)
        {
            Argument.Check("image", image);
            Argument.Check("format", format);
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, format);
                stream.Flush();
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Compresses an image with a given quality.
        /// </summary>
        /// <param name="image">a required image</param>
        /// <param name="quality">a JPEG image quality</param>
        /// <returns>a compressed image buffer</returns>
        public static byte[] ToBytes(Image image, int quality)
        {
            Argument.Check("image", image);
            Argument.CheckLimit("quality", quality, Argument.MORE, -1);
            Argument.CheckLimit("quality", quality, Argument.LESS, 101);
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, StockJpegEncoder, GetQuality(quality));
                stream.Flush();
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Returns a scaled image.
        /// </summary>
        /// <param name="factor">a scaling factor</param>
        /// <param name="image">a required image</param>
        /// <returns>a scaled image</returns>
        public static Image ScaleTo(double factor, Image image)
        {
            Argument.Check("image", image);
            if (factor == 1.0) return image;
            if (factor <= 0.0)
                throw new ArgumentOutOfRangeException("factor", "factor must be > 0.0");

            int width = (int)(image.Width * factor);
            int height = (int)(image.Height * factor);
            Size resultSize = new Size(width, height);
            using (Graphics source = Graphics.FromImage(image))
            {
                Bitmap result = new Bitmap(resultSize.Width, resultSize.Height, source);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, new Rectangle(new Point(), resultSize));
                    return result;
                }
            }
        }

        /// <summary>
        /// Returns an encoder quality parameter.
        /// </summary>
        /// <param name="quality">a quality value</param>
        /// <returns>an encoder quality parameter</returns>
        private static EncoderParameters GetQuality(int quality)
        {
            EncoderParameter jpegQuality = 
                new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            EncoderParameters result = new EncoderParameters(1);
            result.Param[0] = jpegQuality;
            return result;
        }

        private static readonly string JpegMimeType = "image/jpeg";
        private static ImageCodecInfo JpegEncoder = null;

        /// <summary>
        /// A stock JPEG encoder.
        /// </summary>
        private static ImageCodecInfo StockJpegEncoder
        {
            get
            {
                if (JpegEncoder == null)
                    JpegEncoder = FindJpegEncoder();

                return JpegEncoder;
            }
        }

        /// <summary>
        /// Finds the JPEG encoder.
        /// </summary>
        /// <returns>an ImageCodecInfo (or null)</returns>
        /// <remarks>
        /// From Eric White's "Pro .NET 2.0 Graphics Programming", p. 137
        /// </remarks>
        private static ImageCodecInfo FindJpegEncoder()
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo encoder in encoders)
            {
                if (encoder.MimeType == JpegMimeType) return encoder;
            }
            return null;
        }

    } // ImageConversion
}
