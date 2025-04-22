using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Readit.Core.Desktop.Domain
{
    public class ComentariosDesktop : PropertyChangedBase
    {
        public int Id { get; set; }
        public ImageSource ImagemPerfil { get; set; }
        public byte[] ImageByte { get; set; }
        public string UsuarioApelido { get; set; }
        public string TempoUltimaAtualizacaoFormatado { get; set; }
        public DateTime? TempoDecorrido { get; set; }
        public DateTime? TempoUltimaAtualizacaoDecorrido { get; set; }
        public int IdObra { get; set; }
        public int? IdCapitulo { get; set; }
        public int IdUsuario { get; set; }
        public bool IsUsuarioOuAdministrador { get; set; }

        private string _tempoDecorridoFormatado;

        public string TempoDecorridoFormatado
        {
            get => _tempoDecorridoFormatado;
            set
            {
                _tempoDecorridoFormatado = value;
                NotifyOfPropertyChange(() => TempoDecorridoFormatado);
            }
        }

        private string _comentarioTexto;

        public string ComentarioTexto
        {
            get => _comentarioTexto;
            set
            {
                _comentarioTexto = value;
                NotifyOfPropertyChange(() => ComentarioTexto);
            }
        }

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

        private ObservableCollection<ComentariosDesktop> _respostas;

        public ObservableCollection<ComentariosDesktop> Respostas
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

        private bool _isEdicaoComentarioVisivel;

        public bool IsEdicaoComentarioVisivel
        {
            get => _isEdicaoComentarioVisivel;
            set
            {
                _isEdicaoComentarioVisivel = value;
                NotifyOfPropertyChange(() => IsEdicaoComentarioVisivel);
            }
        }

        private bool _isEdicaoRespostaVisivel;

        public bool IsEdicaoRespostaVisivel
        {
            get => _isEdicaoRespostaVisivel;
            set
            {
                _isEdicaoRespostaVisivel = value;
                NotifyOfPropertyChange(() => IsEdicaoRespostaVisivel);
            }
        }

        public ComentariosDesktop Pai { get; set; }

        public ComentariosDesktop()
        {
            Respostas = new ObservableCollection<ComentariosDesktop>();
        }

        public void MostrarResposta()
        {
            IsRespostaVisivel = true;
            NotifyOfPropertyChange(() => IsRespostaVisivel);
        }

        public void MostrarEdicao()
        {
            IsEdicaoComentarioVisivel = true;
            NotifyOfPropertyChange(() => IsEdicaoComentarioVisivel);
        }

        public void MostrarEdicaoResposta()
        {
            IsEdicaoRespostaVisivel = true;
            NotifyOfPropertyChange(() => IsEdicaoRespostaVisivel);
        }

        public void AdicionarResposta(ComentariosDesktop resposta)
        {
            _respostas.Add(resposta);
            NotifyOfPropertyChange(() => Respostas);
        }

        public void RemoverResposta(ComentariosDesktop resposta)
        {
            resposta.Pai.Respostas.Remove(resposta);
            NotifyOfPropertyChange(() => Respostas);
        }
    }
}