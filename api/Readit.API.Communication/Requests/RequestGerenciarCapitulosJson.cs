using Readit.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Communication.Requests
{
    public class RequestGerenciarCapitulosJson
    {
        public List<CapitulosObra> ListaCapitulosObraAdicionar { get; set; } = null!;
        public List<CapitulosObra> ListaCapitulosObraRemover { get; set; } = null!;
    }
}
