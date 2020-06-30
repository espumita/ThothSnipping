using System.Threading.Tasks;

namespace OCRInteroperability {
    public interface ImageToTextConverter {
        Task<string> Convert(byte[] imageData);
    }
}