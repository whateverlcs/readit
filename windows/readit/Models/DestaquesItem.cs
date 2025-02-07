using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace readit.Models
{
    public class DestaquesItem
    {
        public int Rank { get; set; }
        public ImageSource Image { get; set; }
        public byte[] ImageByte { get; set; }
        public string Title { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public double Rating { get; set; }
        public string Filter { get; set; }

        public bool IsLastGenre(string genre)
        {
            return Genres.IndexOf(genre) == Genres.Count - 1;
        }
    }
}
