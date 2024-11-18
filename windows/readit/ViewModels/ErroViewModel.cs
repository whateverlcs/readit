using Caliburn.Micro;

namespace readit.ViewModels
{
    public class ErroViewModel : Screen
    {
        private string _imagem;

        public string Imagem
        {
            get { return _imagem; }
            set
            {
                _imagem = value;
                NotifyOfPropertyChange(() => Imagem);
            }
        }

        private string _mensagem;

        public string Mensagem
        {
            get { return _mensagem; }
            set
            {
                _mensagem = value;
                NotifyOfPropertyChange(() => Mensagem);
            }
        }

        public ErroViewModel(string imagem, string mensagem)
        {
            Imagem = imagem;
            Mensagem = mensagem;
        }
    }
}