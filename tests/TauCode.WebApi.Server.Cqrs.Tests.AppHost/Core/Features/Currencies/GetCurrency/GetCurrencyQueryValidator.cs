using TauCode.Cqrs.Validation;
using TauCode.Validation;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.GetCurrency
{
    public class GetCurrencyQueryValidator : CodedEntityValidator<GetCurrencyQuery, CurrencyCodeValidator>
    {
    }
}
