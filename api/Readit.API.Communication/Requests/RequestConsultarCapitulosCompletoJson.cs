using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Communication.Requests
{
    public class RequestConsultarCapitulosCompletoJson
    {
        public int CapituloId { get; set; }
        public bool IncluirNumeroCapitulos { get; set; }
        public bool IncluirPaginasCapitulo { get; set; }
    }
}
