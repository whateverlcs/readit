using System.Collections.ObjectModel;

namespace Readit.Core.Domain
{
    public class Comentarios
    {
        public int Id { get; set; }
        public byte[] ImageByte { get; set; }
        public string UsuarioApelido { get; set; }
        public string TempoUltimaAtualizacaoFormatado { get; set; }
        public DateTime? TempoDecorrido { get; set; }
        public DateTime? TempoUltimaAtualizacaoDecorrido { get; set; }
        public int IdObra { get; set; }
        public int? IdCapitulo { get; set; }
        public int IdUsuario { get; set; }
        public bool IsUsuarioOuAdministrador { get; set; }
        public string TempoDecorridoFormatado { get; set; }
        public string ComentarioTexto { get; set; }
        public int ContadorLikes { get; set; }
        public int ContadorDislikes { get; set; }
        public ObservableCollection<Comentarios> Respostas { get; set; }
        public Comentarios Pai { get; set; }

        public Comentarios()
        {
            Respostas = new ObservableCollection<Comentarios>();
        }
    }
}