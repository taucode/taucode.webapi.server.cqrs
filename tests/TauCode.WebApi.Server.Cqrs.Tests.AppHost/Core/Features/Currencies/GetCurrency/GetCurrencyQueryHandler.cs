using System.Threading;
using System.Threading.Tasks;
using TauCode.Cqrs.Queries;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies.Exceptions;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.GetCurrency
{
    public class GetCurrencyQueryHandler : IQueryHandler<GetCurrencyQuery>
    {
        private readonly ICurrencyRepository _currencyRepository;

        public GetCurrencyQueryHandler(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public void Execute(GetCurrencyQuery query)
        {
            Currency currency = null;

            if (query.Id != null)
            {
                currency = _currencyRepository.GetById(query.Id);
            }
            else if (query.Code != null)
            {
                currency = _currencyRepository.GetByCode(query.Code);
            }

            if (currency == null)
            {
                throw new CurrencyNotFoundException();
            }

            var queryResult = new GetCurrencyQueryResult
            {
                Id = currency.Id,
                Code = currency.Code,
                Name = currency.Name,
            };

            query.SetResult(queryResult);
        }

        public Task ExecuteAsync(GetCurrencyQuery query, CancellationToken cancellationToken = default)
        {
            this.Execute(query);
            return Task.CompletedTask;
        }
    }
}
