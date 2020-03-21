using TauCode.Cqrs.Commands;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies.Exceptions;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.DeleteCurrency
{
    public class DeleteCurrencyCommandHandler : ICommandHandler<DeleteCurrencyCommand>
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ISession _session;

        public DeleteCurrencyCommandHandler(ICurrencyRepository currencyRepository, ISession session)
        {
            _currencyRepository = currencyRepository;
            _session = session;
        }

        public void Execute(DeleteCurrencyCommand command)
        {
            var isInUse = _session.Query<Quote>().Any(x => x.CurrencyId == command.Id);
            if (isInUse)
            {
                throw new CurrencyIsInUseException();
            }

            var deleted = _currencyRepository.Delete(command.Id);
            if (!deleted)
            {
                throw new CurrencyNotFoundException();
            }
        }
    }
}
