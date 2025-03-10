using Readit.Core.Services;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Readit.Infra.Services
{
    public class ImagemService : IImagemService
    {
        public byte[] ConvertImageToByteArray(string imagePath)
        {
            return File.ReadAllBytes(imagePath);
        }

        public BitmapImage ByteArrayToImage(byte[] imageBytes)
        {
            using (var stream = new MemoryStream(imageBytes))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        public byte[] ConvertBitmapImageToByteArray(ImageSource imageSource)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            return [];
        }
    }
}