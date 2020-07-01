using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using OCRInteroperability;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;

namespace ThothSnipping {
    public partial class MainWindow : Window {
        
        private bool userIsSnipping;
        private Snip snip;
        private ImageToTextConverter imageToTextConverter;

        public MainWindow() {
            imageToTextConverter = OCRFactory.GetOCRSpaceImageToTextConverter();
            InitializeComponent();
            Cursor = Cursors.Cross;
            WindowState = WindowState.Maximized;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs eventArgs) {
            UserStartsSnipping(eventArgs);
        }

        private void UserStartsSnipping(MouseButtonEventArgs eventArgs) {
            userIsSnipping = true;
            var startPosition = eventArgs.GetPosition(this);
            snip = Snip.WithStartPositionIn(startPosition);
        }

        protected override void OnMouseMove(MouseEventArgs eventArgs) {
            if (!userIsSnipping) return;
            RecalculateSnipRectangle(eventArgs);
            ReRenderWindow();
        }

        private void RecalculateSnipRectangle(MouseEventArgs eventArgs) {
            var endPosition = eventArgs.GetPosition(this);
            snip.EndPosition(endPosition);
        }

        private void ReRenderWindow() {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext) {
            if (snip == null) return;
            var pen = new Pen(Brushes.Red, 2);
            var rectangle = snip.Rectangle();
            drawingContext.DrawRectangle(Brushes.Transparent, pen, rectangle);
        }

        protected override async void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs eventArgs) {
            if (!userIsSnipping) return;
            RecalculateSnipRectangle(eventArgs);
            var bitmap = TakeSnipAndCopyToClipBoard();
            await AddTextToClipBoardFrom(bitmap);
            userIsSnipping = false;
            Application.Current.Shutdown();
        }

        private Bitmap TakeSnipAndCopyToClipBoard() {
            Opacity = .0;
            var bitmap = snip.Take();
            Opacity = 0.1;
            ClipboardManager.AddImageToClipBoard(bitmap);
            return bitmap;
        }

        private async Task AddTextToClipBoardFrom(Bitmap bitmap) {
            var text = await TextFrom(bitmap);
            ClipboardManager.AddTextToClipBoard(text);
        }

        private async Task<string> TextFrom(Bitmap bitmap) {
            var imageInByteArray = ConvertToByteArray(bitmap);
            var text = await imageToTextConverter.Convert(imageInByteArray);
            return text;
        }
        
        private static byte[] ConvertToByteArray(Bitmap bitmap) {
            var imageConverter = new ImageConverter();
            return (byte[])imageConverter.ConvertTo(bitmap, typeof(byte[]));
        }
        
    }
}