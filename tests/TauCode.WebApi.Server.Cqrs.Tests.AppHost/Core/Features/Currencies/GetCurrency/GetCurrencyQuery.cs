using TauCode.Cqrs.Queries;
using TauCode.Domain.Identities;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.GetCurrency
{
    public class GetCurrencyQuery : Query<GetCurrencyQueryResult>, ICodedEntityQuery
    {
        public CurrencyId Id { get; set; }
        public string Code { get; set; }

        IdBase ICodedEntityQuery.GetId() => Id;
        string ICodedEntityQuery.GetCode() => Code;
        string ICodedEntityQuery.GetCodePropertyName() => nameof(Code);
    }
}
