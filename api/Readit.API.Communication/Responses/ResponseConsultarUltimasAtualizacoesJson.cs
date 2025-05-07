using Readit.Core.Domain;

namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarUltimasAtualizacoesJson
    {
        public List<UltimasAtualizacoes> UltimasAtualizacoes { get; set; } = null!;
    }

    public class UltimasAtualizacoes
    {
        public int IdObra { get; set; }
        public byte[] Imagem { get; set; }
        public byte[] ImagemFlag { get; set; }
        public string Nome { get; set; }
        public string Status { get; set; }
        public string Tipo { get; set; }
        public List<ChapterInfo> Capitulos { get; set; }
    }
}