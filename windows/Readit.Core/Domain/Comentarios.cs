using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Readit.Core.Domain
{
    public class Comentarios : PropertyChangedBase
    {
        public int Id { get; set; }
        public ImageSource ImagemPerfil { get; set; }
        public byte[] ImageByte { get; set; }
        public string UsuarioApelido { get; set; }
        public string TempoDecorridoFormatado { get; set; }
        public string TempoUltimaAtualizacaoFormatado { get; set; }
        public DateTime? TempoDecorrido { get; set; }
        public DateTime? TempoUltimaAtualizacaoDecorrido { get; set; }
        public string ComentarioTexto { get; set; }
        public int IdObra { get; set; }
        public int? IdCapitulo { get; set; }
        public int IdUsuario { get; set; }

        private int _contadorLikes;

        public int ContadorLikes
        {
            get => _contadorLikes;
            set
            {
                _contadorLikes = value;
                NotifyOfPropertyChange(() => ContadorLikes);
            }
        }

        private int _contadorDislikes;

        public int ContadorDislikes
        {
            get => _contadorDislikes;
            set
            {
                _contadorDislikes = value;
                NotifyOfPropertyChange(() => ContadorDislikes);
            }
        }

        private ObservableCollection<Comentarios> _respostas;

        public ObservableCollection<Comentarios> Respostas
        {
            get => _respostas;
            set
            {
                _respostas = value;
                NotifyOfPropertyChange(() => Respostas);
            }
        }

        private bool _isRespostaVisivel;

        public bool IsRespostaVisivel
        {
            get => _isRespostaVisivel;
            set
            {
                _isRespostaVisivel = value;
                NotifyOfPropertyChange(() => IsRespostaVisivel);
            }
        }

        public Comentarios Pai { get; set; }

        public Comentarios()
        {
            Respostas = new ObservableCollection<Comentarios>();
        }

        public void MostrarResposta()
        {
            IsRespostaVisivel = true;
            NotifyOfPropertyChange(() => IsRespostaVisivel);
        }

        public void AdicionarResposta(Comentarios resposta)
        {
            _respostas.Add(resposta);
            NotifyOfPropertyChange(() => Respostas);
        }
    }
}