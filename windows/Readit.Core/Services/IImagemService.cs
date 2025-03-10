using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Readit.Core.Services
{
    public interface IImagemService
    {
        byte[] ConvertImageToByteArray(string imagePath);

        BitmapImage ByteArrayToImage(byte[] imageBytes);

        byte[] ConvertBitmapImageToByteArray(ImageSource imageSource);
    }
}