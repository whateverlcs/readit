using Readit.API.Application.UseCases.Comentarios.Editar;
using Readit.API.Communication.Requests;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Comentarios.Excluir
{
    public class ExcluirComentarioUseCase
    {
        private readonly IComentarioRepository _comentarioRepository;

        public ExcluirComentarioUseCase(IComentarioRepository comentarioRepository)
        {
            _comentarioRepository = comentarioRepository;
        }

        public async Task<bool> Execute(int idComentario)
        {
            var sucesso = await _comentarioRepository.ExcluirComentarioAsync(idComentario).ConfigureAwait(false);

            if (!sucesso)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a exclusão do comentário, tente novamente em segundos.");
            }

            return sucesso;
        }
    }
}
