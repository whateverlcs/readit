using Readit.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarComentariosJson
    {
        public List<Comentarios> ListaComentarios { get; set; } = null!;
    }
}
