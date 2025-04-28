using System.Net;

namespace Readit.API.Exception
{
    public abstract class ReaditException : SystemException
    {
        public ReaditException(string message) : base(message)
        {
        }

        public abstract List<string> GetErrorMessages();

        public abstract HttpStatusCode GetStatusCode();
    }
}