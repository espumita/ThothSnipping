using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OCRInteroperability;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;

namespace ThothSnipping {
    public partial class MainWindow : Window {
        
        private const double DeltaX = -3.75;
        private const double DeltaY = -3.75;
        private Point startMousePosition;
        private Point endMousePosition;
        private bool IsSnipping;
        private Rect selectedRectangle;
        private ImageToTextConverter imageToTextConverter;

        public MainWindow() {
            imageToTextConverter = OCRFactory.GetOCRSpaceImageToTextConverter();
            InitializeComponent();
            Cursor = Cursors.Cross;
            WindowState = WindowState.Maximized;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs eventArgs) {
            IsSnipping = true;
            startMousePosition = eventArgs.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs eventArgs) {
            if (!IsSnipping) return;
            endMousePosition = eventArgs.GetPosition(this);
            selectedRectangle = new Rect(startMousePosition, endMousePosition);
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext) {
            drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Red, 2), selectedRectangle);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs eventArgs) {
            if (!IsSnipping) return;
            endMousePosition = eventArgs.GetPosition(this);
            selectedRectangle = new Rect(startMousePosition, endMousePosition);
            Snip();                
            IsSnipping = false;
        }

        private async void Snip() {
            using (var bitmap = BitmapWithSelectedRectangleSize()) {
                using (var graphics = Graphics.FromImage(bitmap)) {
                    SnipOf(bitmap.Size, graphics);
                    AddImageToClipBoard(bitmap);
                    var imageAsText = await GetTextFrom(bitmap);
                    AddTextToClipBoard(imageAsText);
                }
            }
        }

        private Bitmap BitmapWithSelectedRectangleSize() {
            var width = Convert.ToInt32(selectedRectangle.Width +  DeltaX);
            var height = Convert.ToInt32(selectedRectangle.Height + DeltaY);
            return new Bitmap(width, height);
        }

        private void SnipOf(Size bitmapSize, Graphics graphics) {
            Opacity = .0;
            var x = Convert.ToInt32(selectedRectangle.X + DeltaX);
            var y = Convert.ToInt32(selectedRectangle.Y + DeltaY);
            graphics.CopyFromScreen(x, y, 0, 0, bitmapSize);
            Opacity = 0.1;
        }

        private async Task<string> GetTextFrom(Bitmap bitmap) {
            var imageInByteArray = ConvertToByteArray(bitmap);
            var text = await imageToTextConverter.Convert(imageInByteArray);
            return text;
        }

        private static void AddImageToClipBoard(Bitmap bitmap) {
            var hBitmap = bitmap.GetHbitmap();
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Clipboard.SetImage(bitmapSource);
        }

        private static byte[] ConvertToByteArray(Bitmap bitmap) {
            var imageConverter = new ImageConverter();
            return (byte[])imageConverter.ConvertTo(bitmap, typeof(byte[]));
        }

        private static void AddTextToClipBoard(string text) {
            Clipboard.SetText(text);
        }
        
    }
}