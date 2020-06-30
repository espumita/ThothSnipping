using OpenOCRApi;

namespace ThothSnipping {

    public class OCRFactory {
        public static OCRSpaceImageToTextConverter GetOCRSpaceImageToTextConverter() {
           return new OCRSpaceImageToTextConverter("helloworld", "eng");
        }
    }
}