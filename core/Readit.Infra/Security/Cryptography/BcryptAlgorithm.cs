using Readit.Core.Domain;
using Readit.Core.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.Infra.Security.Cryptography
{
    public class BcryptAlgorithm : IBcryptAlgorithm
    {
        public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, Usuario user) => BCrypt.Net.BCrypt.Verify(password, user.Senha);
    }
}
