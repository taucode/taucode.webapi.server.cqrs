using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Net.Http;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Exceptions;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.GetCurrency;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.UpdateCurrency;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies.Exceptions;

namespace TauCode.WebApi.Server.Cqrs.Tests.ControllerTests
{
    [TestFixture]
    public class UpdateCurrencyControllerTest : AppHostTestLab
    {
        [Test]
        public void UpdateCurrency_ValidRequest_UpdatesCurrencyAndReturnsOkResultWithUpdatedCurrency()
        {
            // Arrange
            var id = IntegrationTestHelper.PlnId;
            var command = this.CreateCommand();

            // Act
            var response = this.HttpClient.PutAsJsonAsync($"api/currencies/{id}", command).Result;
            var queryResult = response.ReadAs<GetCurrencyQueryResult>();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(queryResult.Id, Is.EqualTo(id));
            Assert.That(queryResult.Code, Is.EqualTo("ZLT"));
            Assert.That(queryResult.Name, Is.EqualTo("Злотуш"));

            var updatedCurrency = this.AssertSession.Query<Currency>().Single(x => x.Id == id);

            Assert.That(updatedCurrency.Code, Is.EqualTo("ZLT"));
            Assert.That(updatedCurrency.Name, Is.EqualTo("Злотуш"));
        }

        [Test]
        public void UpdateCurrency_InvalidRequest_ReturnsValidationErrorResponse()
        {
            // Arrange
            var id = new CurrencyId("59f103f7-c20e-4c97-ac39-17be6adfd3e1");
            var command = new UpdateCurrencyCommand(); // all nulls

            // Act
            var response = this.HttpClient.PutAsJsonAsync($"api/currencies/{id}", command).Result;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var validationError = response.ReadAsValidationError();

            validationError
                .ShouldHaveFailureNumber(2)
                .ShouldContainFailure("code", "NotEmptyValidator", "'Code' must not be empty.")
                .ShouldContainFailure("name", "NotEmptyValidator", "'Name' must not be empty.");
        }

        [Test]
        public void UpdateCurrency_CodeAlreadyExists_ReturnsBusinessLogicErrorResponse()
        {
            // Arrange
            var id = IntegrationTestHelper.NokId;
            var command = this.CreateCommand();
            command.Code = "PLN";

            // Act
            var response = this.HttpClient.PutAsJsonAsync($"api/currencies/{id}", command).Result;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(response.Headers.GetValues("X-Payload-Type").Single(), Is.EqualTo("Error"));

            var error = response.ReadAsError();

            Assert.That(error.Code, Is.EqualTo(typeof(CodeAlreadyExistsException).FullName));
            Assert.That(error.Message, Is.EqualTo("Code already exists."));
        }

        [Test]
        public void UpdateCurrency_CodeAlreadyExistsButSameCurrency_UpdatesCurrencyAndReturnsOkResultWithUpdatedCurrency()
        {
            // Arrange
            var id = IntegrationTestHelper.PlnId;
            var command = new UpdateCurrencyCommand
            {
                Code = "PLN", // keep PLN
                Name = "Duzhe zlotiy",
            };

            // Act
            var response = this.HttpClient.PutAsJsonAsync($"api/currencies/{id}", command).Result;
            var queryResult = response.ReadAs<GetCurrencyQueryResult>();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(queryResult.Id, Is.EqualTo(id));
            Assert.That(queryResult.Code, Is.EqualTo("PLN"));
            Assert.That(queryResult.Name, Is.EqualTo("Duzhe zlotiy"));

            var updatedCurrency = this.AssertSession.Query<Currency>().Single(x => x.Id == id);

            Assert.That(updatedCurrency.Code, Is.EqualTo("PLN"));
            Assert.That(updatedCurrency.Name, Is.EqualTo("Duzhe zlotiy"));
        }

        [Test]
        public void UpdateCurrency_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var nonExistingCurrencyId = new CurrencyId("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
            var command = this.CreateCommand();

            // Act
            var response = this.HttpClient.PutAsJsonAsync($"api/currencies/{nonExistingCurrencyId}", command).Result;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            var error = response.ReadAsError();
            Assert.That(error.Code, Is.EqualTo(typeof(CurrencyNotFoundException).FullName));
            Assert.That(error.Message, Is.EqualTo("Currency not found."));
        }

        private UpdateCurrencyCommand CreateCommand()
        {
            return new UpdateCurrencyCommand
            {
                Code = "ZLT",
                Name = "Злотуш",
            };
        }
    }
}
