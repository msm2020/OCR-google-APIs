# OCR-google-APIs
OCR text detection with google APIs C#, implement text detection using google API with C#, 

Read more on my blog : http://msm2020-sc.blogspot.com.eg/2016/05/ocr-text-detection-with-google-apis-c.html

After searching more than 3 hrs to know how to implement text detection using google API i decided to write this post, hope it help c# developer to know how to use and implement Vision API,

Source Code : https://github.com/msm2020/OCR-google-APIs

1- Create server key on google console

![Image](https://3.bp.blogspot.com/-X62Wx1cjCHU/VzcWhYfqH3I/AAAAAAAAAeg/YSorhlo6HTg970f57jX1gWnPbYT0MhCoQCKgB/s1600/ocr2.png)

* Go to http://console.developers.google.com/ and create new project or use existing project
* From Overview page enable vision API

![Image](https://1.bp.blogspot.com/-EVG99PDiYmw/VzcWhY8pLnI/AAAAAAAAAek/XQjIbgCMri8RxE1JtbIV_H9wxOV4fkPrACKgB/s1600/ocr1.png)

![Image](https://4.bp.blogspot.com/-utJpf1ETBy4/VzcWhreuhhI/AAAAAAAAAeo/JjmBWYVQKa0VOuT7U7KZeLG9xGaVceulgCKgB/s1600/ocr3.png)

* You need to create new bill information to use vision API, it's fully free for 1st 1000 request/ month you can check https://cloud.google.com/vision/docs/pricing
* After Enable Google Cloud Vision API go to Credentials page and create new server key

![Image](https://4.bp.blogspot.com/-hlW2x2og9Hg/VzcWh59dakI/AAAAAAAAAes/YyY_be-BLbYhl2AlLacy5HtjBHE7ElxEQCKgB/s1600/ocr4.png)

* Create key by fill all requested data and download Json file

![Image](https://4.bp.blogspot.com/-ADa0Qb20foc/VzcWhyjXZdI/AAAAAAAAAe8/zNVyHEahAqQpu5l2baghsNui0XdVuthQgCKgB/s1600/ocr5.png)

2- Create Visual studio project I'll use vs 2015 and .NET framework 4.5
* Add google Nuget packages : Google.Apis.Vision.v1 by right click on project and then manage nuget package .

![Image](https://2.bp.blogspot.com/-PmEwRcMhayM/VzcWiNohz-I/AAAAAAAAAe8/E5usRumUp9g_tPxYxUuTo4f9poNm4tuqQCKgB/s1600/ocr6.png)

* Add json file to your project and from properties windows change copy to output directory to true
* Now create new class and add path of your file to use
```
privatestring JsonKeypath
{
     get { return Application.StartupPath + "\\your file name.json"; 
}
```
* We need to create credential object to call with our api
```
private GoogleCredential CreateCredential()
{
    using (var stream = new FileStream(JsonKeypath, FileMode.Open, FileAccess.Read))
          {
              string[] scopes = {  VisionService.Scope.CloudPlatform };
              var credential = GoogleCredential.FromStream(stream);
              credential = credential.CreateScoped(scopes);
              return credential;
}
```
* And create service for vision to request api
```
private VisionService CreateService(GoogleCredential credential)
{
      var service = new VisionService(new  BaseClientService.Initializer()
            {
                 HttpClientInitializer = credential,
                 ApplicationName =ApplicationName,
                 GZipEnabled = true,
            });
      return service;
}
```
* API need language code reference here : https://cloud.google.com/translate/v2/translate-reference#supported_languages
* Create batch request to upload image and get result :
```
BatchAnnotateImagesRequest batchRequest = new BatchAnnotateImagesRequest();
batchRequest.Requests = new List();
batchRequest.Requests.Add(new AnnotateImageRequest()
{
	Features = new List() { new Feature() { Type = "TEXT_DETECTION", MaxResults = 1 }, },
	ImageContext = new ImageContext() { LanguageHints = new List() { language } },
	Image = new Image() { Content = Convert.ToBase64String(file) }
});
```
* Vision api have a lot of type like face detection and other types I use text_detiction https://cloud.google.com/vision/reference/rest/v1/images/annotate#Type
* Excute request to get result
```
var annotate = service.Images.Annotate(batchRequest);
BatchAnnotateImagesResponse batchAnnotateImagesResponse = annotate.Execute();
```
* Response result have Error variable and different type of classes related to Type uses on the api call and will use annotateImageResponse.TextAnnotations to get result and values

Output

![Image](https://4.bp.blogspot.com/-gFFageAE-P0/VzcWi3DToEI/AAAAAAAAAe8/d2vpCVBFrLYSU5_J0UxJOiJ9u0hdqH1qgCKgB/s1600/ocr7.png)

Google reference :
* https://cloud.google.com/vision/docs/
* https://cloud.google.com/vision/reference/rest/v1/images/annotate
