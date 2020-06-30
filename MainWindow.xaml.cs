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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public MainWindow() {
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

        private void Snip() {
            var deltaX = -3.5;
            var deltaY = -3.5;
            using (var bitmap = new Bitmap(Convert.ToInt32(selectedRectangle.Width +  deltaX), Convert.ToInt32(selectedRectangle.Height + deltaY))) {
                using (var graphics = Graphics.FromImage(bitmap)) {
                    String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
                    Opacity = .0;
                    graphics.CopyFromScreen((Convert.ToInt32(selectedRectangle.X + deltaX)), Convert.ToInt32(selectedRectangle.Y + deltaY), 0, 0, bitmap.Size);
                    bitmap.Save("C:\\Screenshots\\" + filename);
                    Opacity = 0.1;
                }
            }
        }
    }
}