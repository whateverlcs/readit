using FluentValidation.Results;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Domain;
using Readit.Core.Enums;
using Readit.Core.Repositories;
using Readit.Core.Security.Cryptography;
using Readit.Core.Security.Tokens.Access;
using Readit.Core.Services;

namespace Readit.API.Application.UseCases.Registro
{
    public class RegistroUsuarioUseCase
    {
        private readonly IBcryptAlgorithm _bcryptAlgorithm;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IImagemService _imagemService;

        public RegistroUsuarioUseCase(IBcryptAlgorithm bcryptAlgorithm, IJwtTokenGenerator jwtTokenGenerator, IUsuarioRepository usuarioRepository, IImagemService imagemService)
        {
            _bcryptAlgorithm = bcryptAlgorithm;
            _jwtTokenGenerator = jwtTokenGenerator;
            _usuarioRepository = usuarioRepository;
            _imagemService = imagemService;
        }

        public async Task<ResponseRegistroUsuarioJson> Execute(RequestUsuarioJson request)
        {
            await Validate(request);

            var usu = new Usuario
            {
                Email = request.Email,
                Nome = request.Nome,
                Apelido = request.Apelido,
                Senha = _bcryptAlgorithm.HashPassword(request.Senha),
                Administrador = false,
            };

            var img = new Imagens
            {
                Imagem = _imagemService.ConvertImageToByteArray(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "profile-default.jpg")),
                Formato = Path.GetExtension(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "profile-default.jpg")),
                Tipo = (byte)EnumObra.TipoImagem.Perfil
            };

            bool sucesso = await _usuarioRepository.CadastrarUsuarioAsync(usu, img, null).ConfigureAwait(false);

            if (!sucesso)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar o cadastro do usuário, tente novamente em segundos.");
            }

            return new ResponseRegistroUsuarioJson
            {
                Nome = usu.Nome,
                AccessToken = _jwtTokenGenerator.Generate(usu)
            };
        }

        private async Task Validate(RequestUsuarioJson request)
        {
            var validator = new RegistroUsuarioValidacao();

            var result = validator.Validate(request);

            var existUserWithEmail = await _usuarioRepository.BuscarUsuarioPorEmailAsync(request.Email);

            if (existUserWithEmail.Count > 0)
                result.Errors.Add(new ValidationFailure("Email", "E-mail já registrado na plataforma"));

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}