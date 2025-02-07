using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace readit.Models
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
