using Azure.Core;
using Readit.API.Application.UseCases.Capitulo.ConsultaCompleta;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Comentarios.Consultar
{
    public class ConsultarComentariosUseCase
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IObraRepository _obraRepository;
        private readonly ICapituloRepository _capituloRepository;

        public ConsultarComentariosUseCase(IComentarioRepository comentarioRepository, IObraRepository obraRepository, ICapituloRepository capituloRepository)
        {
            _comentarioRepository = comentarioRepository;
            _obraRepository = obraRepository;
            _capituloRepository = capituloRepository;
        }

        public async Task<ResponseConsultarComentariosJson> Execute(int idObra, int? idCapitulo)
        {
            await Validate(idObra, idCapitulo);

            var dadosComentarios = await _comentarioRepository.BuscarComentariosObraAsync(idObra, idCapitulo).ConfigureAwait(false);

            return new ResponseConsultarComentariosJson
            {
                ListaComentarios = dadosComentarios
            };
        }

        private async Task Validate(int idObra, int? idCapitulo)
        {
            var obraExistente = await _obraRepository.BuscarObrasPorIdAsync(idObra);

            if (obraExistente.Count == 0)
                throw new NotFoundException("A Obra não existe no sistema.");

            if (idCapitulo != null)
            {
                var capituloExistente = await _capituloRepository.BuscarCapituloObraPorIdAsync((int)idCapitulo);

                if (capituloExistente.Count == 0)
                    throw new NotFoundException("O capítulo não existe no sistema.");
            }
        }
    }
}
