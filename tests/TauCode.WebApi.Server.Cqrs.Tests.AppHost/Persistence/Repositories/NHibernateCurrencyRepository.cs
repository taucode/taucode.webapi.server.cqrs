using System;
using System.Collections.Generic;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Persistence.Repositories
{
    public class NHibernateCurrencyRepository : ICurrencyRepository
    {
        private readonly ISession _session;

        public NHibernateCurrencyRepository(ISession session)
        {
            _session = session;
        }

        public Currency GetById(CurrencyId id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _session.Query<Currency>().SingleOrDefault(x => x.Id == id);
        }

        public Currency GetByCode(string code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            return _session.Query<Currency>().SingleOrDefault(x => x.Code == code);
        }

        public IList<Currency> GetAll()
        {
            return _session.Query<Currency>().ToList();
        }

        public void Save(Currency currency)
        {
            if (currency == null)
            {
                throw new ArgumentNullException(nameof(currency));
            }
            _session.SaveOrUpdate(currency);
        }

        public bool Delete(CurrencyId id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var currency = _session.Query<Currency>().SingleOrDefault(x => x.Id == id);

            if (currency != null)
            {
                _session.Delete(currency);
            }

            return currency != null;
        }
    }
}
