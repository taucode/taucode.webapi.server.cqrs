using NHibernate;
using System.Linq;
using TauCode.Cqrs.Queries;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.GetAllCurrencies
{
    public class GetAllCurrenciesQueryHandler : IQueryHandler<GetAllCurrenciesQuery>
    {
        private readonly ISession _session;

        public GetAllCurrenciesQueryHandler(ISession session)
        {
            _session = session;
        }

        public void Execute(GetAllCurrenciesQuery query)
        {
            var currencies = _session
                .Query<Currency>()
                .OrderBy(x => x.Name)
                .ToList();

            var queryResult = new GetAllCurrenciesQueryResult
            {
                Items = currencies
                    .Select(x => new GetAllCurrenciesQueryResult.CurrencyDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Code = x.Code,
                    })
                    .ToList(),
            };
            query.SetResult(queryResult);
        }
    }
}
