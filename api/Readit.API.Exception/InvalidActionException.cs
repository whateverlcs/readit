using System.Net;

namespace Readit.API.Exception
{
    public class InvalidActionException : ReaditException
    {
        public InvalidActionException(string message) : base(message)
        {
        }

        public override List<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
    }
}