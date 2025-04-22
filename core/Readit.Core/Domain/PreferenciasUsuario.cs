namespace Readit.Core.Domain
{
    public class PreferenciasUsuario
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdPreferencia { get; set; }
        public string Preferencia { get; set; }
    }
}