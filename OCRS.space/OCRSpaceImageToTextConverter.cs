using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OCRInteroperability;

namespace OpenOCRApi {

    public class OCRSpaceImageToTextConverter : ImageToTextConverter {
        private string apiKey;
        private string language;

        public OCRSpaceImageToTextConverter(string apiKey, string language) {
            this.apiKey = apiKey;
            this.language = language;
        }

        public async Task<string> Convert(byte[] imageData) {
            var httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(1, 1, 1);
            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new StringContent(apiKey), "apikey");
            multipartFormDataContent.Add(new StringContent(language), "language");
            multipartFormDataContent.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");
            var response = await httpClient.PostAsync("https://api.ocr.space/Parse/Image", multipartFormDataContent);
            var bodyContent = await response.Content.ReadAsStringAsync();
            var deserializeResponse = JsonConvert.DeserializeObject<OCRSSpaceApiResponse>(bodyContent);
            if (deserializeResponse.OCRExitCode != 1) return $"Error: {deserializeResponse.OCRExitCode}";
            return deserializeResponse.ParsedResults.Select(result => result.ParsedText)
                                             .Aggregate((finalResult,nextResult) => finalResult + nextResult);
        }
    }
}