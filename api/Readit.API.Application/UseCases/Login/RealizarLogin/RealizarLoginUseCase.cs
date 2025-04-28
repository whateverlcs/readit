using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Core.Security.Cryptography;
using Readit.Core.Security.Tokens.Access;

namespace Readit.API.Application.UseCases.Login.FazerLogin
{
    public class RealizarLoginUseCase
    {
        private readonly IBcryptAlgorithm _bcryptAlgorithm;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUsuarioRepository _usuarioRepository;

        public RealizarLoginUseCase(IBcryptAlgorithm bcryptAlgorithm, IJwtTokenGenerator jwtTokenGenerator, IUsuarioRepository usuarioRepository)
        {
            _bcryptAlgorithm = bcryptAlgorithm;
            _jwtTokenGenerator = jwtTokenGenerator;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<ResponseRegistroUsuarioJson> Execute(RequestLoginJson request)
        {
            var entity = await _usuarioRepository.BuscarUsuarioPorEmailAsync(request.Email);

            if (entity.Count == 0)
            {
                throw new InvalidLoginException();
            }

            var passwordIsValid = _bcryptAlgorithm.Verify(request.Password, entity[0]);

            if (passwordIsValid == false)
                throw new InvalidLoginException();

            return new ResponseRegistroUsuarioJson
            {
                Nome = entity[0].Nome,
                AccessToken = _jwtTokenGenerator.Generate(entity[0])
            };
        }
    }
}