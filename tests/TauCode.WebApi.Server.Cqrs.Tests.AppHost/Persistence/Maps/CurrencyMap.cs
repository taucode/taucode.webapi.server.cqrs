using FluentNHibernate.Mapping;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Persistence.Maps
{
    public class CurrencyMap : ClassMap<Currency>
    {
        public CurrencyMap()
        {
            this.Id(x => x.Id);
            this.Map(x => x.Code);
            this.Map(x => x.Name);
        }
    }
}
