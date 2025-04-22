using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Readit.Core.Desktop.Services
{
    public interface IImagemDesktopService
    {
        BitmapImage ByteArrayToImage(byte[] imageBytes);

        byte[] ConvertBitmapImageToByteArray(ImageSource imageSource);
    }
}