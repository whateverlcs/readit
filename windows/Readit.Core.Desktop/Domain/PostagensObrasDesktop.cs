using Readit.Core.Domain;
using System.Windows.Media;

namespace Readit.Core.Desktop.Domain
{
    public class PostagensObrasDesktop : PostagensObras
    {
        public ImageSource Image { get; set; }
        public ImageSource ImageFlag { get; set; } //Imagem das bandeiras dos paises de acordo com o tipo da obra
    }
}