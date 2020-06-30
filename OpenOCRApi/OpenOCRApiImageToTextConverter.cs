using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OCRInteroperability;

namespace OpenOCRApi {

    public class OpenOCRApiImageToTextConverter : ImageToTextConverter {
        
        public async Task<string> Convert(byte[] imageData) {
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(1, 1, 1);


            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent("helloworld"), "apikey"); //Added api key in form data
            form.Add(new StringContent("eng"), "language");
            //form.Add(new StringContent("spa"), "language");
            form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");

            HttpResponseMessage response = await httpClient.PostAsync("https://api.ocr.space/Parse/Image", form);

            string strContent = await response.Content.ReadAsStringAsync();



            Rootobject ocrResult = JsonConvert.DeserializeObject<Rootobject>(strContent);

            var result = "";
            if (ocrResult.OCRExitCode == 1) {
                for (int i = 0; i < ocrResult.ParsedResults.Count(); i++) {
                    result = result + ocrResult.ParsedResults[i].ParsedText;
                }
            } else {
               Console.WriteLine($"ERROR {ocrResult.OCRExitCode}");
            }
            return result;
        }
    }
    
    public class Rootobject {
        public Parsedresult[] ParsedResults { get; set; }
        public int OCRExitCode { get; set; }
        public bool IsErroredOnProcessing { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
    }

    public class Parsedresult {
        public object FileParseExitCode { get; set; }
        public string ParsedText { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
    }
}