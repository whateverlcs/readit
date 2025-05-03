using Readit.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarCapitulosCompletosJson
    {
        public List<CapitulosObra> ListaCapitulosObras { get; set; } = null!;
        public CapitulosObra CapituloObra { get; set; } = null!;
    }
}
