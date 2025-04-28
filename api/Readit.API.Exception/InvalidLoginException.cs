using System.Net;

namespace Readit.API.Exception
{
    public class InvalidLoginException : ReaditException
    {
        public InvalidLoginException() : base("Email e/ou senha inválidos.")
        {
        }

        public override List<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
    }
}