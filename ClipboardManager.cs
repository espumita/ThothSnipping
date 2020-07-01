using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ThothSnipping {
    public static class ClipboardManager {
        
        public static void AddImageToClipBoard(Bitmap bitmap) {
            var hBitmap = bitmap.GetHbitmap();
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Clipboard.SetImage(bitmapSource);
        }

        public static void AddTextToClipBoard(string text) {
            Clipboard.SetText(text);
        }
    }
}