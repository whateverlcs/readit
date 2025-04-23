using Microsoft.IdentityModel.Tokens;
using Readit.Core.Domain;
using Readit.Core.Security.Tokens.Access;
using Readit.Core.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Readit.Infra.Security.Tokens.Access
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguracaoService _configuracaoService;

        public JwtTokenGenerator(IConfiguracaoService configuracaoService)
        {
            _configuracaoService = configuracaoService;
        }

        public string Generate(Usuario user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(60),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(SecurityKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }

        public SymmetricSecurityKey SecurityKey()
        {
            var signingKey = _configuracaoService.GetAPIKey();

            var symmetricKey = Encoding.UTF8.GetBytes(signingKey);

            return new SymmetricSecurityKey(symmetricKey);
        }
    }
}
