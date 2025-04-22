namespace Readit.Core.Domain
{
    public class PostagensObras
    {
        public int ObraId { get; set; }
        public byte[] ImageByte { get; set; }
        public string Title { get; set; } // Titulo que pode ou não ser abreviado de acordo com o seu tamanho
        public string TitleOriginal { get; set; }
        public string Status { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public int TipoNumber { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public double Rating { get; set; }
        public DateTime DataPublicacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
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