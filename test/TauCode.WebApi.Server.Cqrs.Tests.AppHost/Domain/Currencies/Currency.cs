using System;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies
{
    public class Currency
    {
        private Currency()
        {
        }

        public Currency(string code, string name)
        {
            this.Id = new CurrencyId();
            this.ChangeCode(code);
            this.ChangeName(name);
        }

        public CurrencyId Id { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }

        public void ChangeCode(string code)
        {
            this.Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public void ChangeName(string name)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
