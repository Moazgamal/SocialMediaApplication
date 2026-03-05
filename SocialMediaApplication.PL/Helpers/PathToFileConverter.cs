using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace SocialMediaApplication.PL.Helpers
{
    public static class PathToFileConverter
    {
        public static IFormFile ConvertPathToFile(string imageName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot/files/images",
                            imageName);

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(imageName, out string contentType))
            {
                contentType = "application/octet-stream"; // fallback
            }

            return new FormFile(stream, 0, stream.Length, "file", imageName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
    }
}
