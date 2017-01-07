using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Innostar.UI
{
    public static class ImageHelper
    {
        private static Dictionary<string, ImageCodecInfo> _encoders = null;

        public static Dictionary<string, ImageCodecInfo> Encoders
        {
            get
            {
                if (_encoders == null)
                {
                    _encoders = new Dictionary<string, ImageCodecInfo>();
                }

                if (_encoders.Count == 0)
                {
                    foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                    {
                        _encoders.Add(codec.MimeType.ToLower(), codec);
                    }
                }

                return _encoders;
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var result = new Bitmap(width, height);

            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var lookupKey = mimeType.ToLower();

            ImageCodecInfo foundCodec = null;

            if (Encoders.ContainsKey(lookupKey))
            {
                foundCodec = Encoders[lookupKey];
            }

            return foundCodec;
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            Image returnImage;

            using (var ms = new MemoryStream(byteArrayIn))
            {
                returnImage = Image.FromStream(ms);
            }

            return returnImage;
        }

        public static byte[] ImageToByteArray(Image image, int quality)
        {
            using (var ms = new MemoryStream())
            {
                if ((quality < 0) || (quality > 100))
                {
                    var error = string.Format("Jpeg image quality must be between 0 and 100, with 100 being the highest quality.  A value of {0} was specified.", quality);

                    throw new ArgumentOutOfRangeException(error);
                }

                var qualityParam = new EncoderParameter(Encoder.Quality, quality);

                var codec = GetEncoderInfo("image/png");

                var encoderParams = new EncoderParameters(1);

                encoderParams.Param[0] = qualityParam;
                image.Save(ms, codec, encoderParams);

                return ms.ToArray();
            }
        }

        public static byte[] ImageToByteArray(Image image)
        {
            var ms = new MemoryStream();

            image.Save(ms, image.RawFormat);
            var result = ms.ToArray();

            return result;
        }

        public static byte[] ResizeImage(Image image, Size size, int quality)
        {
            var resizedImage = ResizeImage(image, size.Width, size.Height);

            return ImageToByteArray(resizedImage, quality);
        }
    }
}