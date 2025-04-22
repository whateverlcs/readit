namespace Readit.Core.Domain
{
    public class DetalhesObra
    {
        public int ObraId { get; set; }
        public byte[] ImageByte { get; set; }
        public string Title { get; set; }
        public string[] Tags { get; set; } // Generos da obra
        public double Rating { get; set; }
        public double RatingUsuario { get; set; }
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