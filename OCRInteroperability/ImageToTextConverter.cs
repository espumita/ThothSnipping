using System.Drawing;

namespace OCRInteroperability {
    public interface ImageToTextConverter {
        string Convert(Bitmap bitmap);
    }
}