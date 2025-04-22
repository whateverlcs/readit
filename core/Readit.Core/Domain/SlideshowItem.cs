namespace Readit.Core.Domain
{
    public class SlideshowItem
    {
        public string Chapter { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; } // Generos da Obra
        public byte[] BackgroundImageByte { get; set; }
        public byte[] FocusedImageByte { get; set; }
    }
}