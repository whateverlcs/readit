using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Communication.Requests
{
    public class RequestConsultarCapitulosSimplesJson
    {
        public List<int> ListaNumerosCapitulos { get; set; } = null!;
    }
}
