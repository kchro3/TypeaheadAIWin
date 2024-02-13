using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Forms;

namespace TypeaheadAIWin.Source
{
    internal static class ScreenshotUtil
    {
        public static Bitmap CaptureFullScreen()
        {
            var screenSize = SystemInformation.PrimaryMonitorSize;
            var bitmap = new Bitmap(screenSize.Width, screenSize.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, screenSize);
            }
            return bitmap;
        }

        public static Bitmap? CaptureArea(int x, int y, int width, int height, int maxWidth = 1024)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("Capture area dimensions must be greater than zero.");
            }

            try
            {
                Bitmap bmp = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
                }

                if (width > maxWidth)
                {
                    bmp = ResizeBitmap(bmp, maxWidth);
                }

                return bmp;
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the details)
                Trace.WriteLine("Error capturing screen area: " + ex.Message);
                return null;
            }
        }

        public static Bitmap ResizeBitmap(Bitmap original, int width)
        {
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            float ratio = (float)width / originalWidth;
            int newHeight = (int)(originalHeight * ratio);

            Bitmap newImage = new Bitmap(width, newHeight);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.DrawImage(original, 0, 0, width, newHeight);
            }

            return newImage;
        }

        public static ImageSource? ConvertBitmapToImageSource(Bitmap? bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap), "Bitmap cannot be null.");
            }

            try
            {
                using var memory = new MemoryStream();
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the details)
                Trace.WriteLine("Error converting bitmap to ImageSource: " + ex.Message);
                return null;
            }
        }
    }
}
