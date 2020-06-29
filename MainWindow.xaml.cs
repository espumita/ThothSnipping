using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ThothSnipping {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private Point startMousePosition;
        private Point endMousePosition;
        private bool IsSnipping;

        public MainWindow() {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs eventArgs) {
            IsSnipping = true;
            startMousePosition = eventArgs.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs eventArgs) {
            if (IsSnipping) {
                endMousePosition = eventArgs.GetPosition(this);
                InvalidateVisual();
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs eventArgs) {
            if (IsSnipping) {
                endMousePosition = eventArgs.GetPosition(this);
                Snip();                
                IsSnipping = false;
            }
        }

        protected override void OnRender(DrawingContext drawingContext) {
            if (startMousePosition != null) {
                drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Red, 2), new Rect(startMousePosition, endMousePosition));
            }
        }

        private void Snip() {
            Console.WriteLine("Snip!!!");
        }
    }
}