using Caliburn.Micro;
using Readit.Core.Domain;
using System.Windows.Media;

namespace Readit.Core.Desktop.Domain
{
    public class DetalhesObraDesktop : PropertyChangedBase
    {
        public int ObraId { get; set; }
        public ImageSource Image { get; set; }
        public ImageSource ImageFlag { get; set; } //Imagem das bandeiras dos paises de acordo com o tipo da obra
        public byte[] ImageByte { get; set; }
        public string Title { get; set; }
        public string[] Tags { get; set; } // Generos da obra

        private double _rating;

        public double Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                NotifyOfPropertyChange(() => Rating);
            }
        }

        private double _ratingUsuario;

        public double RatingUsuario
        {
            get => _ratingUsuario;
            set
            {
                _ratingUsuario = value;
                NotifyOfPropertyChange(() => RatingUsuario);
            }
        }

        public string Description { get; set; }
        public string Status { get; set; }
        public int StatusNumber { get; set; } //Status da Obra no formato de Number
        public string Type { get; set; } //Tipo da obra formatado em string
        public int TypeNumber { get; set; } //Tipo da obra no formato de Number
        public string PostedBy { get; set; }
        public string SeriesReleasedDate { get; set; }
        public string SeriesLastUpdatedDate { get; set; }
        public int Views { get; set; }
        public bool Bookmark { get; set; }
        public List<ChapterInfo> ChapterInfos { get; set; }
    }
}