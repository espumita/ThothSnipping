using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OCRInteroperability;
using OpenOCRApi;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace ThothSnipping {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private Point startMousePosition;
        private Point endMousePosition;
        private bool IsSnipping;
        private Rect selectedRectangle;
        private ImageToTextConverter imageToTextConverter;

        public MainWindow() {
            imageToTextConverter = new OpenOCRApiImageToTextConverter();
            InitializeComponent();
            Cursor = Cursors.Cross;
            WindowState = WindowState.Maximized;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs eventArgs) {
            IsSnipping = true;
            startMousePosition = eventArgs.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs eventArgs) {
            if (IsSnipping) {
                endMousePosition = eventArgs.GetPosition(this);
                selectedRectangle = new Rect(startMousePosition, endMousePosition);
                InvalidateVisual();
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs eventArgs) {
            if (IsSnipping) {
                endMousePosition = eventArgs.GetPosition(this);
                selectedRectangle = new Rect(startMousePosition, endMousePosition);
                Snip();                
                IsSnipping = false;
            }
        }

        protected override void OnRender(DrawingContext drawingContext) {
            if (startMousePosition != null) {
                drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Red, 2), selectedRectangle);
            }
        }

        private async void Snip() {
            var deltaX = -3.75;
            var deltaY = -3.75;
            using (var bitmap = new Bitmap(Convert.ToInt32(selectedRectangle.Width +  deltaX), Convert.ToInt32(selectedRectangle.Height + deltaY))) {
                using (var graphics = Graphics.FromImage(bitmap)) {
                    String filename = "ThothSnipping-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
                    Opacity = .0;
                    graphics.CopyFromScreen((Convert.ToInt32(selectedRectangle.X + deltaX)), Convert.ToInt32(selectedRectangle.Y + deltaY), 0, 0, bitmap.Size);
                    bitmap.Save("C:\\Users\\david\\Desktop\\Screenshots\\" + filename);
                    Opacity = 0.1;
                    AddSelectedImageToClipBoard(bitmap);
                    var imageConverter = new ImageConverter();
                    var imageData = (byte[])imageConverter.ConvertTo(bitmap, typeof(byte[]));
                    var text = await imageToTextConverter.Convert(imageData);
                    Console.WriteLine(text);
                    AddSelectedImageTextToClipBoard(text);
                }
            }
        }

        private static void AddSelectedImageToClipBoard(Bitmap bitmap) {
            var intPtr = bitmap.GetHbitmap();
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Clipboard.SetImage(bitmapSource);
        }
        
        private static void AddSelectedImageTextToClipBoard(string text) {
            Clipboard.SetText(text);
        }
    }
}