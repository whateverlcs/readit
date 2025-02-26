using Caliburn.Micro;
using readit.Controls;
using readit.Data;
using readit.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace readit.ViewModels
{
    public class DetalhamentoObraViewModel : Screen
    {
        private bool _exibirMenuAdministrador = Global.UsuarioLogado.Administrador;

        public bool ExibirMenuAdministrador
        {
            get { return _exibirMenuAdministrador; }
            set
            {
                _exibirMenuAdministrador = value;
                NotifyOfPropertyChange(() => ExibirMenuAdministrador);
            }
        }

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                NotifyOfPropertyChange(() => Loading);
            }
        }

        public string _nomeObra;

        #region Detalhes da Obra

        private DetalhesObra _dadosDetalhesObra;

        public DetalhesObra DadosDetalhesObra
        {
            get { return _dadosDetalhesObra; }
            set
            {
                _dadosDetalhesObra = value;
                NotifyOfPropertyChange(() => DadosDetalhesObra);
                AtualizarEstrelas();
            }
        }

        public ICommand GenreClickCommand { get; set; }

        public ICommand StarClickedCommand { get; set; }

        public ICommand ChapterClickedCommand { get; set; }

        public ObservableCollection<StarRating> StarRatings { get; set; }

        #endregion Detalhes da Obra

        #region Tooltip Message

        private string _msgInfo;

        public string MsgInfo
        {
            get { return _msgInfo; }
            set
            {
                _msgInfo = value;
                NotifyOfPropertyChange(() => MsgInfo);
            }
        }

        private string _corMsgInfo = "#000000";

        public string CorMsgInfo
        {
            get { return _corMsgInfo; }
            set
            {
                _corMsgInfo = value;
                NotifyOfPropertyChange(() => CorMsgInfo);
            }
        }

        private bool _exibirMensagem;

        public bool ExibirMensagem
        {
            get { return _exibirMensagem; }
            set
            {
                _exibirMensagem = value;
                NotifyOfPropertyChange(() => ExibirMensagem);
            }
        }

        #endregion Tooltip Message

        #region Capitulos Obra

        private string _searchQuery;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                NotifyOfPropertyChange(() => SearchQuery);
                FiltrarCapitulos();
            }
        }

        private List<ChapterInfo> _filteredCapitulos;

        public List<ChapterInfo> FilteredCapitulos
        {
            get => _filteredCapitulos;
            set
            {
                _filteredCapitulos = value;
                NotifyOfPropertyChange(() => FilteredCapitulos);
            }
        }

        #endregion Capitulos Obra

        #region Loading

        private Visibility _exibirSecoes;

        public Visibility ExibirSecoes
        {
            get { return _exibirSecoes; }
            set
            {
                _exibirSecoes = value;
                NotifyOfPropertyChange(() => ExibirSecoes);
            }
        }

        private List<string> _texts;
        private int _index, _textIndex;
        private bool _removing;
        private DispatcherTimer _timer;

        private string _animatedText;

        public string AnimatedText
        {
            get => _animatedText;
            set
            {
                _animatedText = value;
                NotifyOfPropertyChange(() => AnimatedText);
            }
        }

        #endregion Loading

        private ControlPrincipal cp = new ControlPrincipal();

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public DetalhamentoObraViewModel(string nomeObra)
        {
            _nomeObra = nomeObra;

            AplicarLoading(true);

            Thread thread = new(CarregarDadosObraThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            GenreClickCommand = new RelayCommand<string>(OnGenreClick);
            StarClickedCommand = new RelayCommand<object>(StarClicked);
            ChapterClickedCommand = new RelayCommand<ChapterInfo>(OnChapterClick);
        }

        public void AplicarLoading(bool loading)
        {
            if (loading)
            {
                Loading = true;
                ExibirSecoes = Visibility.Hidden;

                _texts = cp.ExtrairDadosFrasesLoading();
                _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
                _timer.Tick += (s, e) =>
                {
                    var currentText = _texts[_textIndex];
                    AnimatedText = _removing ? currentText[..(--_index)] : currentText[..(++_index)];

                    if (_index == currentText.Length) _removing = true;
                    if (_index == 0)
                    {
                        _removing = false;
                        _textIndex = (_textIndex + 1) % _texts.Count;
                    }
                };
                _timer.Start();
            }
            else
            {
                try
                {
                    _timer.Stop();
                }
                catch { }

                Loading = false;
                ExibirSecoes = Visibility.Visible;
            }
        }

        public void CarregarDadosObraThread()
        {
            db.AtualizarViewObra(_nomeObra);
            DadosDetalhesObra = cp.FormatarDadosDetalhamentoObra(_nomeObra);
            FilteredCapitulos = DadosDetalhesObra.ChapterInfos;

            AplicarLoading(false);
        }

        private void FiltrarCapitulos()
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                FilteredCapitulos = DadosDetalhesObra.ChapterInfos;
            }
            else
            {
                FilteredCapitulos = DadosDetalhesObra.ChapterInfos
                    .Where(c => c.Chapter.ToLower().Contains(SearchQuery.ToLower()))
                    .ToList();
            }
        }

        public void InicializarEstrelas()
        {
            StarRatings = new ObservableCollection<StarRating>();
            for (int i = 0; i < 5; i++)
            {
                StarRatings.Add(new StarRating { StarIndex = i, IsFilled = false });
            }
        }

        public void AtualizarEstrelas()
        {
            if (DadosDetalhesObra == null) return;

            if (StarRatings == null)
            {
                InicializarEstrelas();
            }

            int fullStars = (int)Math.Round(DadosDetalhesObra.Rating); // Arredonda para preencher corretamente
            for (int i = 0; i < StarRatings.Count; i++)
            {
                StarRatings[i].IsFilled = i < fullStars;
            }
            NotifyOfPropertyChange(() => StarRatings);
        }

        private void StarClicked(object param)
        {
            if (param is int index && DadosDetalhesObra != null)
            {
                DadosDetalhesObra.Rating = index + 1; // Atualiza o Rating com base no clique do usuário
                AtualizarEstrelas();
                db.AtualizarRating(DadosDetalhesObra.ObraId, DadosDetalhesObra.Rating);
                NotifyOfPropertyChange(() => DadosDetalhesObra);
            }
        }

        public void StarHovered(int index)
        {
            for (int i = 0; i < StarRatings.Count; i++)
            {
                StarRatings[i].IsHovered = i <= index;
            }
        }

        public void StarHoverExit()
        {
            foreach (var star in StarRatings)
            {
                star.IsHovered = false;
            }
        }

        private void OnGenreClick(string genre)
        {
            // Lógica ao clicar em um gênero
            string err = "";
        }

        private void OnChapterClick(ChapterInfo chapter)
        {
            // Lógica ao clicar em um gênero
            string err = "";
        }

        public void RealizarBookmarkObra()
        {
            Thread thread = new(RealizarBookmarkObraThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void RealizarBookmarkObraThread()
        {
            var sucesso = db.CadastrarRemoverBookmark(new BookmarksUsuario { UsuarioId = Global.UsuarioLogado.Id, ObraId = DadosDetalhesObra.ObraId });

            if (sucesso.Item1)
            {
                DadosDetalhesObra.Bookmark = sucesso.Item2 == "Adicionado";
                NotifyOfPropertyChange(() => DadosDetalhesObra);

                _ = ExibirMensagemFlashAsync("Sucesso", [$"{(DadosDetalhesObra.Bookmark ? "Adicionado aos" : "Removido dos")} Bookmarks com sucesso!"]);
            }
            else
            {
                _ = ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao salvar/remover o bookmark."]);
            }
        }

        public async Task ExibirMensagemFlashAsync(string tipoMensagem, List<string> mensagens)
        {
            foreach (var mensagem in mensagens)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MsgInfo = mensagem;
                    CorMsgInfo = tipoMensagem switch
                    {
                        "Informação" => "#7c94fa",
                        "Sucesso" => "#70a757",
                        "Erro" => "#d24330",
                        _ => "#000000"
                    };
                    ExibirMensagem = true;
                });

                await Task.Delay(4000);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ExibirMensagem = false;
                });
            }
        }

        public void SelecionarCadastro()
        {
            _ = ActiveView.OpenItemMain(new SelecaoCadastroViewModel());
        }

        public void PaginaInicial()
        {
            _ = ActiveView.OpenItemMain(new PaginaInicialViewModel());
        }

        public void EditarPerfil()
        {
            _ = ActiveView.OpenItemMain(new EditarPerfilViewModel());
        }

        public void BookmarksUsuario()
        {
            _ = ActiveView.OpenItemMain(new BookmarksViewModel());
        }

        public void ListagemObras()
        {
            _ = ActiveView.OpenItemMain(new ListagemObrasViewModel());
        }
    }
}