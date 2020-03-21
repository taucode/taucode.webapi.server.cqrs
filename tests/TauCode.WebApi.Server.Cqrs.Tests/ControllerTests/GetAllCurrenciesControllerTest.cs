using NUnit.Framework;
using System.Linq;
using System.Net;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.GetAllCurrencies;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;

namespace TauCode.WebApi.Server.Cqrs.Tests.ControllerTests
{
    [TestFixture]
    public class GetAllCurrenciesControllerTest : AppHostTest
    {
        [Test]
        public void GetAllCurrencies_NoArguments_ReturnsAllCurrencies()
        {
            // Arrange

            // Act
            var response = this.HttpClient.GetAsync($"api/currencies").Result;

            var queryResult = response.ReadAs<GetAllCurrenciesQueryResult>();
            var currencies = queryResult
                .Items
                .OrderBy(x => x.Code)
                .ToList();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(currencies, Has.Count.EqualTo(163));

            // check some currencies
            var currency = currencies[6];
            Assert.That(currency.Id, Is.EqualTo(new CurrencyId("7274f0b6-4028-4663-a5b1-3b2082750011")));
            Assert.That(currency.Code, Is.EqualTo("ARS"));
            Assert.That(currency.Name, Is.EqualTo("Argentine peso"));

            currency = currencies[77];
            Assert.That(currency.Id, Is.EqualTo(new CurrencyId("47b46c6d-b981-4994-8867-d92331897273")));
            Assert.That(currency.Code, Is.EqualTo("KPW"));
            Assert.That(currency.Name, Is.EqualTo("North Korean won"));
        }
    }
}
