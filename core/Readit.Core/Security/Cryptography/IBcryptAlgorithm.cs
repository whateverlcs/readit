using Readit.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.Core.Security.Cryptography
{
    public interface IBcryptAlgorithm
    {
        public string HashPassword(string password);
        public bool Verify(string password, Usuario user);
    }
}
