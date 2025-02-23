using System.Windows.Media;

namespace readit.Models
{
    public class PostagensObras
    {
        public int ObraId { get; set; }
        public ImageSource Image { get; set; }
        public byte[] ImageByte { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int StatusNumber { get; set; }
        public List<ChapterInfo> ChapterInfos { get; set; }
    }

    public class ChapterInfo
    {
        public int ChapterId { get; set; }
        public int ObraId { get; set; }
        public string Chapter { get; set; }
        public string TimeAgo { get; set; }
        public DateTime TimeAgoDate { get; set; }
    }
}