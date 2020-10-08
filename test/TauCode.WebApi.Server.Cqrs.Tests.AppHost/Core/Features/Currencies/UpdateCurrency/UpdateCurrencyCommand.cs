using TauCode.Cqrs.Abstractions;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.UpdateCurrency
{
    public class UpdateCurrencyCommand : ICommand
    {
        public CurrencyId Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
