using TauCode.Cqrs.Commands;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.CreateCurrency
{
    public class CreateCurrencyCommand : Command<CurrencyId>
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
