using Caliburn.Micro;
using System.Windows.Media;

namespace Readit.Core.Domain
{
    public class DetalhesObra : PropertyChangedBase
    {
        public int ObraId { get; set; }
        public ImageSource Image { get; set; }
        public byte[] ImageByte { get; set; }
        public string Title { get; set; }
        public string[] Tags { get; set; }

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

        public string Description { get; set; }
        public string Status { get; set; }
        public int StatusNumber { get; set; }
        public string Type { get; set; }
        public int TypeNumber { get; set; }
        public string PostedBy { get; set; }
        public string SeriesReleasedDate { get; set; }
        public string SeriesLastUpdatedDate { get; set; }
        public int Views { get; set; }
        public bool Bookmark { get; set; }
        public List<ChapterInfo> ChapterInfos { get; set; }
    }
}