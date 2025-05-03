using Readit.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Communication.Requests
{
    public class RequestConsultarAvaliacoesJson
    {
        public int IdComentario { get; set; }
        public string TipoAvaliacao { get; set; } = string.Empty;
    }
}
