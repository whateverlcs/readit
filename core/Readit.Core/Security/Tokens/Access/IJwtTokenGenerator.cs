using Microsoft.IdentityModel.Tokens;
using Readit.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.Core.Security.Tokens.Access
{
    public interface IJwtTokenGenerator
    {
        public string Generate(Usuario user);
        public SymmetricSecurityKey SecurityKey();
    }
}
