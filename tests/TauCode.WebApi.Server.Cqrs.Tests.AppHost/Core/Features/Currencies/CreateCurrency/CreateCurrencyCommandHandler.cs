using TauCode.Cqrs.Commands;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.CreateCurrency
{
    public class CreateCurrencyCommandHandler : ICommandHandler<CreateCurrencyCommand>
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CreateCurrencyCommandHandler(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public void Execute(CreateCurrencyCommand command)
        {
            var existingCurrency = _currencyRepository.GetByCode(command.Code);
            if (existingCurrency != null)
            {
                throw new CodeAlreadyExistsException();
            }

            var currency = new Currency(command.Code, command.Name);
            _currencyRepository.Save(currency);
            command.SetResult(currency.Id);
        }
    }
}
