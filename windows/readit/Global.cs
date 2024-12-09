using readit.Models;

namespace readit
{
    public static class Global
    {
        public static Usuario UsuarioLogado { get; set; }
        public static List<string> ListaCapitulosSelecionados { get; set; } = new List<string>();
    }
}