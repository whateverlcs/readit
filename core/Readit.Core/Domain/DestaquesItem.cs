namespace Readit.Core.Domain
{
    public class DestaquesItem
    {
        public int Rank { get; set; }
        public byte[] ImageByte { get; set; }
        public string Title { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public double Rating { get; set; }
        public string Filter { get; set; } // O filtro sendo diário, semanal e mensal.
        public int TipoNumber { get; set; } //Tipo da obra no formato de Number

        public bool IsLastGenre(string genre)
        {
            return Genres.IndexOf(genre) == Genres.Count - 1;
        }
    }
}