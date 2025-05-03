using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Communication.Requests
{
    public class RequestGerenciarAvaliacoesComentarioJson
    {
        public int ComentarioId { get; set; }
        public string TipoAvaliacao { get; set; } = string.Empty;
        public string TipoAcao { get; set; } = string.Empty;
    }
}
