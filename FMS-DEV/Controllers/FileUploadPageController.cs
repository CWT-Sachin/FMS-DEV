//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Drive.v3;
//using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;


namespace YourApplication.Controllers
{
    public class FileUploadPageController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Confirm()
        {
            // Perform cancellation logic here
            return View("FileUploadConfirmed");
        }
    }
}

////connecting Google API KEY

//namespace DriveV3Snippets
//{
//    // Class to demonstrate Drive's upload with conversion use-case.
//    public class UploadWithConversion
//    {
//        /// <summary>
//        /// Upload file with conversion.
//        /// </summary>
//        /// <param name="filePath">Id of the spreadsheet file.</param>
//        /// <returns>Inserted file id if successful, null otherwise.</returns>
//        public static string DriveUploadWithConversion(string filePath)
//        {
//            try
//            {
//                /* Load pre-authorized user credentials from the environment.
//                 TODO(developer) - See https://developers.google.com/identity for
//                 guides on implementing OAuth2 for your application. */
//                GoogleCredential credential = GoogleCredential.GetApplicationDefault()
//                    .CreateScoped(DriveService.Scope.Drive);

//                // Create Drive API service.
//                var service = new DriveService(new BaseClientService.Initializer
//                {
//                    HttpClientInitializer = credential,
//                    ApplicationName = "Drive API Snippets"
//                });

//                // Upload file My Report on drive.
//                var fileMetadata = new Google.Apis.Drive.v3.Data.File();

//                fileMetadata.Name = Path.GetFileName(filePath);
//                fileMetadata.MimeType = "image/jpeg";


//                FilesResource.CreateMediaUpload request;
//                // Create a new drive.
//                using (var stream = new FileStream(filePath,
//                           FileMode.Open))

//                {
//                    // Create a new file, with metadata and stream.
//                    request = service.Files.Create(
//                        fileMetadata, stream, "image/jpeg");
//                    request.Fields = "id";
//                    request.Upload();

//                    // Create a new file, with metadata and stream.
//                    request = service.Files.Create(
//                        fileMetadata, stream, "text/csv");
//                    request.Fields = "id";
//                    request.Upload();
//                }

//                var file = request.ResponseBody;
//                // Prints the uploaded file id.
//                Console.WriteLine("File ID: " + file.Id);
//                return file.Id;
//            }
//            catch (Exception e)
//            {
//                // TODO(developer) - handle error appropriately
//                if (e is AggregateException)
//                {
//                    Console.WriteLine("Credential Not found");
//                }
//                else if (e is FileNotFoundException)
//                {
//                    Console.WriteLine("File not found");
//                }
//                else
//                {
//                    throw;
//                }
//            }
//            return null;
//        }
//    }
//}






