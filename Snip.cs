using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;

namespace ThothSnipping {
    public class Snip {
        private const double DeltaX = -3.75;
        private const double DeltaY = -3.75;
        private readonly Point startPosition;
        private Point endPosition;

        private Snip(Point startPosition) {
            this.startPosition = startPosition;
        }

        public static Snip WithStartPositionIn(Point startPosition) {
            return new Snip(startPosition);
        }

        public void EndPosition(Point endPosition) {
            this.endPosition = endPosition;
        }

        public Rect Rectangle() {
            return new Rect(startPosition, endPosition);
        }

        public Bitmap Take() {
            var snipRectangle = Rectangle();
            var bitmap = BitmapWithSelectedRectangleSize(snipRectangle);
            using (var graphics = Graphics.FromImage(bitmap)) {
                SnipOf(bitmap.Size, graphics, snipRectangle);
            }
            return bitmap;
        }
        
        private static Bitmap BitmapWithSelectedRectangleSize(Rect snipRectangle) {
            var width = Convert.ToInt32(snipRectangle.Width +  DeltaX);
            var height = Convert.ToInt32(snipRectangle.Height + DeltaY);
            return new Bitmap(width, height);
        }

        private static void SnipOf(Size size, Graphics graphics, Rect snipRectangle) {
            var x = Convert.ToInt32(snipRectangle.X + DeltaX);
            var y = Convert.ToInt32(snipRectangle.Y + DeltaY);
            graphics.CopyFromScreen(x, y, 0, 0, size);
        }

    }
}