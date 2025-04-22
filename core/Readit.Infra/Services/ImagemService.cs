using Readit.Core.Services;

namespace Readit.Infra.Services
{
    public class ImagemService : IImagemService
    {
        public byte[] ConvertImageToByteArray(string imagePath)
        {
            return File.ReadAllBytes(imagePath);
        }
    }
}