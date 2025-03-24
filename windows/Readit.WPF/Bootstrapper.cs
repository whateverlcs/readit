using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Data.Context;
using Readit.Data.Repositories;
using Readit.Data.Services;
using Readit.Infra.Configuration;
using Readit.Infra.Logging;
using Readit.Infra.Services;
using Readit.WPF.Infrastructure;
using Readit.WPF.ViewModels;
using System.IO;
using System.Windows;

namespace Readit.WPF
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            LoggingConfig.ConfigureLogging();
            Initialize();
        }

        protected override void Configure()
        {
            // Configuração do appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Registro das Configurações
            _container.Instance<IConfiguration>(configuration);

            // Criar uma instância do ConfiguracaoService
            var configuracaoService = new ConfiguracaoService(configuration);
            _container.Instance<IConfiguracaoService>(configuracaoService);

            // Obter a string de conexão do ConfiguracaoService
            var connectionString = configuracaoService.GetConnectionString();

            // Configurar o IDbContextFactory com a string de conexão
            var serviceProvider = new ServiceCollection()
                .AddDbContextFactory<ReaditContext>(options =>
                    options.UseSqlServer(connectionString))
                .BuildServiceProvider();

            // Registrar o IDbContextFactory no SimpleContainer
            _container.Instance(serviceProvider.GetRequiredService<IDbContextFactory<ReaditContext>>());

            // Registro os serviços principais do Caliburn.Micro
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();

            // Registro Context
            _container.PerRequest<ReaditContext>();

            // Registro Serviços
            _container.Singleton<IUsuarioService, UsuarioService>();
            _container.Singleton<IDatabaseService, DatabaseService>();
            _container.Singleton<ILoggingService, SerilogLogger>();
            _container.Singleton<IArquivoService, ArquivoService>();
            _container.Singleton<ICapituloService, CapituloService>();
            _container.Singleton<IEnumService, EnumService>();
            _container.Singleton<IImagemService, ImagemService>();
            _container.Singleton<IObraService, ObraService>();
            _container.Singleton<IUtilService, UtilService>();
            _container.Singleton<IComentarioService, ComentarioService>();

            // Registro Repositórios
            _container.Singleton<IAvaliacaoObraRepository, AvaliacaoObraRepository>();
            _container.Singleton<IBookmarkRepository, BookmarkRepository>();
            _container.Singleton<ICapituloRepository, CapituloRepository>();
            _container.Singleton<IGeneroRepository, GeneroRepository>();
            _container.Singleton<IImagemRepository, ImagemRepository>();
            _container.Singleton<IObraRepository, ObraRepository>();
            _container.Singleton<IPaginaCapituloRepository, PaginaCapituloRepository>();
            _container.Singleton<IUsuarioRepository, UsuarioRepository>();
            _container.Singleton<IVisualizacaoObraRepository, VisualizacaoObraRepository>();
            _container.Singleton<IComentarioRepository, ComentarioRepository>();

            // Registro ViewModels
            _container.PerRequest<BookmarksViewModel, BookmarksViewModel>();
            _container.PerRequest<CadastroCapituloViewModel, CadastroCapituloViewModel>();
            _container.PerRequest<CadastroGeneroViewModel, CadastroGeneroViewModel>();
            _container.PerRequest<CadastroObraViewModel, CadastroObraViewModel>();
            _container.PerRequest<CadastroViewModel, CadastroViewModel>();
            _container.PerRequest<DetalhamentoObraViewModel, DetalhamentoObraViewModel>();
            _container.PerRequest<EditarPerfilViewModel, EditarPerfilViewModel>();
            _container.PerRequest<ErroViewModel, ErroViewModel>();
            _container.PerRequest<LeituraCapituloViewModel, LeituraCapituloViewModel>();
            _container.PerRequest<ListagemObrasViewModel, ListagemObrasViewModel>();
            _container.PerRequest<LoginViewModel, LoginViewModel>();
            _container.PerRequest<PaginaInicialViewModel, PaginaInicialViewModel>();
            _container.PerRequest<SelecaoCadastroViewModel, SelecaoCadastroViewModel>();
            _container.PerRequest<ShellMainViewModel, ShellMainViewModel>();
            _container.PerRequest<ShellViewModel, ShellViewModel>();

            // Registrar o ServiceProvider
            DependencyResolver.SetContainer(_container);
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null) return instance;
            throw new InvalidOperationException($"Não pode resolver {service.Name}");
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<ShellViewModel>();
        }
    }
}