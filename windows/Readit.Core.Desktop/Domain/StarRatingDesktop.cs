using Caliburn.Micro;

namespace Readit.Core.Desktop.Domain
{
    public class StarRatingDesktop : PropertyChangedBase
    {
        public int StarIndex { get; set; }

        private bool _isFilled;

        public bool IsFilled
        {
            get => _isFilled;
            set
            {
                _isFilled = value;
                NotifyOfPropertyChange(() => IsFilled);
            }
        }

        private bool _isHovered;

        public bool IsHovered
        {
            get => _isHovered;
            set
            {
                _isHovered = value;
                NotifyOfPropertyChange(() => IsHovered);
            }
        }
    }
}