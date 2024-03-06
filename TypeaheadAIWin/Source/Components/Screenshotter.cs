using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace TypeaheadAIWin.Source.Components
{
    public class Screenshotter
    {
        public Bitmap? CaptureArea(int x, int y, int width, int height, int maxWidth = 1024)
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

        public Bitmap ResizeBitmap(Bitmap original, int width)
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

        public ImageSource? ConvertBitmapToImageSource(Bitmap? bitmap)
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
