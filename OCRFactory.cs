using System;
using System.Net.Http;
using OpenOCRApi;

namespace ThothSnipping {

    public class OCRFactory {
        public static OCRSpaceImageToTextConverter GetOCRSpaceImageToTextConverter() {
            var httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(1, 1, 1);
            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new StringContent("helloworld"), "apikey");
            multipartFormDataContent.Add(new StringContent("eng"), "language");
            return new OCRSpaceImageToTextConverter(httpClient, multipartFormDataContent);
        }
    }
}