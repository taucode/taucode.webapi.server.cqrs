using System;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Exceptions
{
    [Serializable]
    public class CodeAlreadyExistsException : Exception
    {
        public CodeAlreadyExistsException()
            : this("Code already exists.")
        {
        }

        public CodeAlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}
