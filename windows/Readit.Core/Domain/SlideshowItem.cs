using Caliburn.Micro;
using System.Windows.Media;

namespace Readit.Core.Domain
{
    public class SlideshowItem : PropertyChangedBase
    {
        public string Chapter { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public ImageSource BackgroundImage { get; set; }
        public byte[] BackgroundImageByte { get; set; }
        public ImageSource FocusedImage { get; set; }
        public byte[] FocusedImageByte { get; set; }

        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                NotifyOfPropertyChange(() => IsActive);
            }
        }
    }
}