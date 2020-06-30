using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OCRInteroperability;

namespace OpenOCRApi {

    public class OCRSpaceImageToTextConverter : ImageToTextConverter {
        private const string OCRSpaceApiUri = "https://api.ocr.space/Parse/Image";
        private readonly HttpClient httpClient;
        private readonly MultipartFormDataContent multipartFormDataContent;

        public OCRSpaceImageToTextConverter(HttpClient httpClient, MultipartFormDataContent multipartFormDataContent) {
            this.httpClient = httpClient;
            this.multipartFormDataContent = multipartFormDataContent;
        }

        public async Task<string> Convert(byte[] imageData) {
            var requestBodyContent = AddImageToMultipartFormDataContent(imageData, multipartFormDataContent);
            var response = await httpClient.PostAsync(OCRSpaceApiUri, requestBodyContent);
            var responseBodyContent = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<OCRSSpaceApiResponse>(responseBodyContent);
            return TextFrom(deserializedResponse);
        }

        private static MultipartFormDataContent AddImageToMultipartFormDataContent(byte[] imageData, MultipartFormDataContent requestContent) {
            var byteArrayContent = new ByteArrayContent(imageData, 0, imageData.Length);
            requestContent.Add(byteArrayContent, "image", "image.jpg");
            return requestContent;
        }

        private static string TextFrom(OCRSSpaceApiResponse response) {
            if (response.OCRExitCode != 1) return $"Error: {response.OCRExitCode}";
            return response.ParsedResults.Select(result => result.ParsedText)
                                         .Aggregate((finalResult,nextResult) => finalResult + nextResult);
        }
    }
}