﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Exception
{
    public class ForbiddenOperationException : ReaditException
    {
        public ForbiddenOperationException(string message) : base(message)
        {
        }

        public override List<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.Forbidden;
    }
}
