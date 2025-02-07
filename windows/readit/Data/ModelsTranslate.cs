using readit.Enums;
using readit.Models;
using ef = EntityFramework_DB.Models;

namespace readit.Data
{
    public class ModelsTranslate
    {
        public ef.Usuario UsuarioModelToDB(Usuario usuario)
        {
            ef.Usuario usu = new ef.Usuario
            {
                UsuId = usuario.Id,
                UsuNome = usuario.Nome,
                UsuApelido = usuario.Apelido,
                UsuEmail = usuario.Email,
                UsuSenha = usuario.Senha,
                UsuAdministrador = usuario.Administrador,
                ImgId = usuario.IdImagem
            };

            return usu;
        }

        public ef.TipoVisualizacaoObraUsuario TipoVisualizacaoObraUsuarioModelToDB(TipoVisualizacaoObraUsuario tipoVisualizacaoObraUsuario)
        {
            ef.TipoVisualizacaoObraUsuario tvou = new ef.TipoVisualizacaoObraUsuario
            {
                TvouId = tipoVisualizacaoObraUsuario.Id,
                TvoId = tipoVisualizacaoObraUsuario.TipoVisualizacaoObraId,
                UsuId = tipoVisualizacaoObraUsuario.UsuarioId,
            };

            return tvou;
        }

        public ef.Imagen ImagemModelToDB(Imagens imagem)
        {
            ef.Imagen img = new ef.Imagen
            {
                ImgId = imagem.Id,
                ImgFormato = imagem.Formato,
                ImgDataInclusao = imagem.DataInclusao,
                ImgDataAtualizacao = imagem.DataAtualizacao,
                ImgImagem = imagem.Imagem,
                ImgTipo = imagem.Tipo
            };

            return img;
        }

        public ef.Genero GeneroModelToDB(Generos genero)
        {
            ef.Genero gen = new ef.Genero
            {
                GnsId = genero.Id,
                GnsNome = genero.Nome,
            };

            return gen;
        }

        public ef.Obra ObraModelToDB(Obras obra)
        {
            ef.Obra ob = new ef.Obra
            {
                ObsId = obra.Id,
                ObsNomeObra = obra.NomeObra,
                ObsStatus = obra.Status,
                ObsTipo = obra.Tipo,
                ObsDescricao = obra.Descricao,
                ObsDataPublicacao = obra.DataPublicacao,
                ObsDataAtualizacao = obra.DataAtualizacao,
                UsuId = obra.UsuarioId,
                ImgId = obra.ImagemId
            };

            return ob;
        }

        public ef.CapitulosObra CapitulosObraDBToModel(CapitulosObra capObra)
        {
            ef.CapitulosObra capObraDB = new ef.CapitulosObra
            {
                CpoId = capObra.Id,
                CpoNumeroCapitulo = capObra.NumeroCapitulo,
                CpoDataPublicacao = capObra.DataPublicacao,
                CpoDataAtualizacao = capObra.DataAtualizacao,
                UsuId = capObra.UsuarioId,
                ObsId = capObra.ObraId
            };

            return capObraDB;
        }

        public ef.PaginasCapitulo PaginasCapituloDBToModel(PaginasCapitulo pagCap)
        {
            ef.PaginasCapitulo pagCapDB = new ef.PaginasCapitulo
            {
                PgcId = pagCap.Id,
                PgcNumeroPagina = pagCap.NumeroPagina,
                PgcPagina = pagCap.Pagina,
                PgcTamanho = pagCap.Tamanho,
                CpoId = pagCap.CapituloId
            };

            return pagCapDB;
        }

        public List<Usuario> UsuariosDBToModel(ef.Usuario[] usuariosDB)
        {
            List<Usuario> listaUsuarios = [];

            foreach (var usuDB in usuariosDB)
            {
                Usuario usu = new Usuario
                {
                    Id = usuDB.UsuId,
                    Nome = usuDB.UsuNome,
                    Apelido = usuDB.UsuApelido,
                    Email = usuDB.UsuEmail,
                    Senha = usuDB.UsuSenha,
                    Administrador = usuDB.UsuAdministrador,
                    IdImagem = usuDB.ImgId
                };

                listaUsuarios.Add(usu);
            }

            return listaUsuarios;
        }

        public List<Obras> ObrasDBToModel(ef.Obra[] obrasDB)
        {
            List<Obras> listaObras = [];

            foreach (var obraDB in obrasDB)
            {
                Obras obra = new Obras
                {
                    Id = obraDB.ObsId,
                    NomeObra = obraDB.ObsNomeObra,
                    Status = obraDB.ObsStatus,
                    Tipo = obraDB.ObsTipo,
                    Descricao = obraDB.ObsDescricao,
                    DataAtualizacao = obraDB.ObsDataAtualizacao,
                    DataPublicacao = obraDB.ObsDataPublicacao,
                    ImagemId = obraDB.ImgId,
                    UsuarioId = obraDB.UsuId
                };

                listaObras.Add(obra);
            }

            return listaObras;
        }

        public List<SlideshowItem> ObrasSlideShowDBToModel(dynamic[] obrasDB)
        {
            List<SlideshowItem> listaObras = [];

            foreach (var obraDB in obrasDB)
            {
                SlideshowItem obra = new SlideshowItem
                {
                    Title = obraDB.NomeObra,
                    Chapter = $"Capítulo: {obraDB.UltimoCapitulo.ToString("D2")}",
                    Description = obraDB.Descricao,
                    Tags = obraDB.Generos,
                    BackgroundImageByte = obraDB.Imagem,
                    FocusedImageByte = obraDB.Imagem,
                };

                listaObras.Add(obra);
            }

            return listaObras;
        }

        public List<PostagensObras> ObrasUltimasAtualizacoesDBToModel(dynamic[] obrasDB)
        {
            List<PostagensObras> listaObras = [];

            foreach (var obraDB in obrasDB)
            {
                PostagensObras obra = new PostagensObras
                {
                    ImageByte = obraDB.Imagem,
                    Title = obraDB.NomeObra,
                    StatusNumber = obraDB.Status,
                };

                obra.ChapterInfos = new List<ChapterInfo>();

                foreach (var cap in obraDB.Capitulos)
                {
                    ChapterInfo chap = new ChapterInfo
                    {
                        Chapter = $"• Capítulo {cap.Numero.ToString("D2")}",
                        TimeAgoDate = cap.Data
                    };

                    obra.ChapterInfos.Add(chap);
                }

                listaObras.Add(obra);
            }

            return listaObras;
        }

        public List<CapitulosObra> CapitulosObrasDBToModel(ef.CapitulosObra[] capituloObrasDB)
        {
            List<CapitulosObra> listaCapitulosObras = [];

            foreach (var capObraDB in capituloObrasDB)
            {
                CapitulosObra capituloObra = new CapitulosObra
                {
                    Id = capObraDB.CpoId,
                    NumeroCapitulo = capObraDB.CpoNumeroCapitulo,
                    DataPublicacao = capObraDB.CpoDataPublicacao,
                    DataAtualizacao = capObraDB.CpoDataAtualizacao,
                    UsuarioId = capObraDB.UsuId,
                    ObraId = capObraDB.ObsId
                };

                listaCapitulosObras.Add(capituloObra);
            }

            return listaCapitulosObras;
        }

        public List<PaginasCapitulo> PaginasCapituloDBToModel(ef.PaginasCapitulo[] paginasCapituloDB)
        {
            List<PaginasCapitulo> listaPaginasCapitulo = [];

            foreach (var pagCapDB in paginasCapituloDB)
            {
                PaginasCapitulo paginaCapitulo = new PaginasCapitulo
                {
                    Id = pagCapDB.PgcId,
                    NumeroPagina = pagCapDB.PgcNumeroPagina,
                    Pagina = pagCapDB.PgcPagina,
                    Tamanho = pagCapDB.PgcTamanho,
                    CapituloId = pagCapDB.CpoId
                };

                listaPaginasCapitulo.Add(paginaCapitulo);
            }

            return listaPaginasCapitulo;
        }

        public List<Generos> GenerosDBToModel(ef.Genero[] generosDB)
        {
            List<Generos> listaGeneros = [];

            foreach (var generoDB in generosDB)
            {
                Generos genero = new Generos
                {
                    Id = generoDB.GnsId,
                    Nome = generoDB.GnsNome,
                };

                listaGeneros.Add(genero);
            }

            return listaGeneros;
        }

        public List<Imagens> ImagensDBToModel(ef.Imagen[] imagensDB)
        {
            List<Imagens> listaImagens = [];

            foreach (var imagemDB in imagensDB)
            {
                Imagens img = new Imagens
                {
                    Id = imagemDB.ImgId,
                    Imagem = imagemDB.ImgImagem,
                    Formato = imagemDB.ImgFormato,
                    DataInclusao = imagemDB.ImgDataInclusao,
                    DataAtualizacao = imagemDB.ImgDataAtualizacao,
                    Tipo = imagemDB.ImgTipo,
                };

                listaImagens.Add(img);
            }

            return listaImagens;
        }

        public List<TipoVisualizacaoObra> TipoVisualizacaoObraDBToModel(ef.TipoVisualizacaoObra[] tipoVisualizacaoObraDB)
        {
            List<TipoVisualizacaoObra> listaTiposVisualizacaoObra = [];

            foreach (var tvoDB in tipoVisualizacaoObraDB)
            {
                TipoVisualizacaoObra tvo = new TipoVisualizacaoObra
                {
                    Id = tvoDB.TvoId,
                    Nome = tvoDB.TvoVisualizacao == (int)EnumObra.TipoVisualizacaoObra.PaginaInteira ? "Página Inteira" : "Por Página",
                    Visualizacao = tvoDB.TvoVisualizacao,
                };

                listaTiposVisualizacaoObra.Add(tvo);
            }

            return listaTiposVisualizacaoObra;
        }

        public List<TipoVisualizacaoObraUsuario> TipoVisualizacaoObraUsuarioDBToModel(ef.TipoVisualizacaoObraUsuario[] tipoVisualizacaoObraUsuarioDB)
        {
            List<TipoVisualizacaoObraUsuario> listaTiposVisualizacaoObraUsuario = [];

            foreach (var tvouDB in tipoVisualizacaoObraUsuarioDB)
            {
                TipoVisualizacaoObraUsuario tvou = new TipoVisualizacaoObraUsuario
                {
                    Id = tvouDB.TvouId,
                    UsuarioId = tvouDB.UsuId,
                    TipoVisualizacaoObraId = tvouDB.TvoId,
                };

                listaTiposVisualizacaoObraUsuario.Add(tvou);
            }

            return listaTiposVisualizacaoObraUsuario;
        }
    }
}