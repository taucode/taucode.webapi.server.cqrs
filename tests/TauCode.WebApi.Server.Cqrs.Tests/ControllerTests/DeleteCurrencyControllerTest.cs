using System.Net;
using NUnit.Framework;

namespace TauCode.WebApi.Server.Cqrs.Tests.ControllerTests
{
    [TestFixture]
    public class DeleteCurrencyControllerTest : ThisAppHostTestBase
    {
        [Test]
        public void Delete_ExistingId_DeletesAndReturnsNoContentResultWithId()
        {
            // Arrange
            // create fictional currency
            CurrencyId id = null;

            this.SetupSession.DoInTransaction(() =>
            {
                var currency = new Currency("WAT", "Wat Dynero");
                this.SetupSession.Save(currency);
                id = currency.Id;
            });

            // Act
            var response = this.HttpClient.DeleteAsync($"api/currencies/{id}").Result;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var deleteResult = response.ReadAs<DeleteResultDto>();
            
            Assert.That(deleteResult.InstanceId, Is.EqualTo(id.ToString()));

            
            var deletedCurrency = this.AssertSession
                .Query<Currency>()
                .SingleOrDefault(x => x.Id == id);

            Assert.That(deletedCurrency, Is.Null);
        }

        [Test]
        public void Delete_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var nonExistingCurrencyId = new CurrencyId("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

            // Act
            var response = this.HttpClient.DeleteAsync($"api/currencies/{nonExistingCurrencyId}").Result;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            var error = response.ReadAsError();
            Assert.That(error.Code, Is.EqualTo(typeof(CurrencyNotFoundException).FullName));
            Assert.That(error.Message, Is.EqualTo("Currency not found."));
        }

        [Test]
        public void Delete_IsInUse_ReturnsBusinessLogicErrorResult()
        {
            // Arrange
            var usedCurrencyId = IntegrationTestHelper.UsdId;

            // Act
            var response = this.HttpClient.DeleteAsync($"api/currencies/{usedCurrencyId}").Result;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(response.Headers.GetValues("X-Payload-Type").Single(), Is.EqualTo("Error"));

            var error = response.ReadAsError();

            Assert.That(error.Code, Is.EqualTo(typeof(CurrencyIsInUseException).FullName));
            Assert.That(error.Message, Is.EqualTo("Currency is in use."));
        }
    }
}
