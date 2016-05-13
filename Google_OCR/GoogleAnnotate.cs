using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using Newtonsoft.Json;

namespace Google_Orc
{
    public class GoogleAnnotate
    {
        public string ApplicationName { get { return "Ocr"; } }
        public string JsonResult { get; set; }
        public string TextResult { get; set; }
        public string Error { get; set; }

        private string JsonKeypath
        {
            //get { return Application.StartupPath + "\\your file name.json"; }
            get { return Application.StartupPath + "\\key.json"; }
        }

        private GoogleCredential _credential;
        private GoogleCredential CreateCredential()
        {
            if (_credential != null) return _credential;
            using (var stream = new FileStream(JsonKeypath, FileMode.Open, FileAccess.Read))
            {
                string[] scopes = { VisionService.Scope.CloudPlatform };
                var credential = GoogleCredential.FromStream(stream);
                credential = credential.CreateScoped(scopes);
                _credential = credential;
                return credential;
            }
        }

        private VisionService CreateService(GoogleCredential credential)
        {
            var service = new VisionService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
                GZipEnabled = true,
            });
            return service;
        }


        public void GetText(string imgPath, string language)
        {
            TextResult = JsonResult = "";
            byte[] file = File.ReadAllBytes(imgPath);

            var credential = CreateCredential();
            var service = CreateService(credential);
            service.HttpClient.Timeout = new TimeSpan(1, 1, 1);

            BatchAnnotateImagesRequest batchRequest = new BatchAnnotateImagesRequest();
            batchRequest.Requests = new List<AnnotateImageRequest>();
            batchRequest.Requests.Add(new AnnotateImageRequest()
            {
                Features = new List<Feature>() { new Feature() { Type = "TEXT_DETECTION", MaxResults = 1 }, },
                ImageContext = new ImageContext() { LanguageHints = new List<string>() { language } },
                Image = new Image() { Content = Convert.ToBase64String(file) }
            });

            var annotate = service.Images.Annotate(batchRequest);
            BatchAnnotateImagesResponse batchAnnotateImagesResponse = annotate.Execute();
            if (batchAnnotateImagesResponse.Responses.Any())
            {
                AnnotateImageResponse annotateImageResponse = batchAnnotateImagesResponse.Responses[0];
                if (annotateImageResponse.Error != null)
                {
                    if (annotateImageResponse.Error.Message != null)
                        Error = annotateImageResponse.Error.Message;
                }
                else
                {
                    if (annotateImageResponse.TextAnnotations != null && annotateImageResponse.TextAnnotations.Any())
                    {
                        TextResult = annotateImageResponse.TextAnnotations[0].Description.Replace("\n", "\r\n");
                        JsonResult = JsonConvert.SerializeObject(annotateImageResponse.TextAnnotations[0]);
                    }
                    return;

                }

            }

            return;
            //return TextResult;
        }

    }
}
