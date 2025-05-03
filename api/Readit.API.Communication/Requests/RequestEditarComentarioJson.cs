using Readit.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Communication.Requests
{
    public class RequestEditarComentarioJson
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public int ObraId { get; set; }
        public int? CapituloId { get; set; }
        public int UsuarioId { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }
}
