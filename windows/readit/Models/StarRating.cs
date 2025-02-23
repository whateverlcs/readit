using Caliburn.Micro;

namespace readit.Models
{
    public class StarRating : PropertyChangedBase
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