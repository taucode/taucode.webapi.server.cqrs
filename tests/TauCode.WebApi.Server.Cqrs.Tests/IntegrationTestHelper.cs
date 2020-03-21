using NHibernate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;

namespace TauCode.WebApi.Server.Cqrs.Tests
{
    internal static class IntegrationTestHelper
    {
        internal static readonly CurrencyId UsdId = new CurrencyId("ce49c7a1-66f8-494e-b5cd-9b9b925637ee");
        internal static readonly CurrencyId UahId = new CurrencyId("c419319b-dfa4-48dc-b42a-bb7d2c58ce5a");
        internal static readonly CurrencyId EurId = new CurrencyId("fe781e20-5fd3-4ec6-9350-dd1859649e14");
        internal static readonly CurrencyId PlnId = new CurrencyId("e82dd292-5bea-4e8b-8359-d373ae4cea2a");
        internal static readonly CurrencyId UgxId = new CurrencyId("6dfe0c94-b1c2-4e4f-a933-fdcd70280954");
        internal static readonly CurrencyId NokId = new CurrencyId("b8a080f6-2cf3-48fa-b276-2b990f3456ca");
        internal static readonly CurrencyId AfnId = new CurrencyId("f68dbcf3-9a1e-4123-bfa2-6effa58d2f1d");
        internal static readonly CurrencyId DkkId = new CurrencyId("cc4ece2c-f7ba-408b-a263-5c7f522b4762");
        internal static readonly CurrencyId CadId = new CurrencyId("b3669345-61b6-4400-8c25-2a254ffbf5db");

        internal static DateTime? ToDateTime(this string s)
        {
            if (s == null)
            {
                return null;
            }

            return DateTime.Parse(s, CultureInfo.InvariantCulture);
        }

        internal static IEnumerable<string> GetAllCurrencyCodes(ISession session)
        {
            var codes = session
                .Query<Currency>()
                .ToList()
                .Select(x => x.Code);
            return codes;
        }
    }
}