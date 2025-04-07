namespace Readit.Core.Domain
{
    public class Preferencias
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool IsSelected { get; set; } // Utilizado no combobox de preferencias
    }
}