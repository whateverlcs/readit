namespace readit.Enums
{
    public static class EnumObra
    {
        public enum StatusObra
        {
            EmAndamento = 1,
            EmHiato = 2,
            Finalizado = 3,
            Cancelado = 4,
            Dropado = 5
        }

        public enum TipoObra
        {
            Manhwa = 1,
            Donghua = 2,
            Manga = 3
        }

        public enum TipoImagem
        {
            Perfil = 1,
            Obra = 2
        }

        public enum TipoVisualizacaoObra
        {
            PaginaInteira = 1,
            PorPagina = 2
        }

        public enum TipoOrdenacao
        {
            OrdemAlfabetica = 1,
            OrdemAlfabeticaReversa = 2,
            OrdemObrasRecemAtualizadas = 3,
            OrdemPublicacao = 4,
            OrdemMaisPopulares = 5
        }
    }
}