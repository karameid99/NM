using DM.Core.DTOs.General;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Image
{
    public class ImageService : IImageService
    {
        private static ImageOptimizer _optimizer = new ImageOptimizer();
        private readonly ImageSettings _imageSettings;

        public ImageService(IOptions<ImageSettings> imageSettings)
        {
            _imageSettings = imageSettings.Value;
        }

        public async Task<string> Save(IFormFile file, string Foldername)
        {
            var baseURL = _imageSettings.WebSite;
            string fileFullPath = null;
            if (file != null && file.Length > 0)
            {
                var uploadPath = Path.Combine(_imageSettings.RootPath, Foldername);
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                    uploadPath = Path.Combine(uploadPath, fileName);
                    fileFullPath = baseURL + "//" + Path.Combine(Foldername, fileName);
                    var dir = Path.GetDirectoryName(uploadPath);
                    Directory.CreateDirectory(dir);
                    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    CompressImage(uploadPath);
                }
            }
            return fileFullPath;
        }

        private async void CompressImage(string imagePath)
        {
            await Task.Run(() => _optimizer.Compress(imagePath));
        }

    }
}
