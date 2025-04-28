using System.Net;

namespace Readit.API.Exception
{
    public class NotFoundException : ReaditException
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public override List<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.NotFound;
    }
}