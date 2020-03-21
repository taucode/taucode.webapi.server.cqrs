using System;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies.Exceptions
{
    [Serializable]
    public class CurrencyNotFoundException : Exception
    {
        public CurrencyNotFoundException()
            : this("Currency not found.")
        {
        }

        public CurrencyNotFoundException(string message)
            : base(message)
        {
        }
    }
}
