using NUnit.Framework;
using System.Net;

namespace TauCode.WebApi.Server.Cqrs.Tests.ControllerTests
{
    [TestFixture]
    public class CreateCurrencyControllerTest : AppHostTest
    {
        [Test]
        public void CreateCurrency_ValidRequest_CreatesCurrencyAndReturnsCreatedResultWithCreatedCurrency()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            var response = this.HttpClient.PostAsJsonAsync($"api/currencies", command).Result;
            var createResultDto = response.ReadAs<CreateResultDto<GetCurrencyQueryResult>>();
            var queryResult = createResultDto.Content;
            var id = queryResult.Id;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Headers.GetValues("X-Payload-Type").Single(), Is.EqualTo("CreateResult"));

            Assert.That(createResultDto.InstanceId, Is.EqualTo(id.ToString()));
            Assert.That(createResultDto.Url, Is.EqualTo($"/api/currencies/by-prop?id={id}"));

            Assert.That(queryResult.Code, Is.EqualTo(command.Code));
            Assert.That(queryResult.Name, Is.EqualTo(command.Name));

            var createdCurrency = this.AssertSession.Load<Currency>(id);

            Assert.That(createdCurrency.Code, Is.EqualTo(command.Code));
            Assert.That(createdCurrency.Name, Is.EqualTo(command.Name));
        }

        [Test]
        public void CreateCurrency_InvalidRequest_ReturnsValidationErrorResponse()
        {
            // Arrange
            var command = this.CreateCommand();
            command.Code = null;
            command.Name = null;

            // Act
            var response = this.HttpClient.PostAsJsonAsync($"api/currencies", command).Result;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var validationError = response.ReadAsValidationError();

            validationError
                .ShouldHaveFailureNumber(2)
                .ShouldContainFailure("code", "NotEmptyValidator", "'Code' must not be empty.")
                .ShouldContainFailure("name", "NotEmptyValidator", "'Name' must not be empty.");
        }

        [Test]
        public void CreateCurrency_CodeAlreadyExists_ReturnsBusinessLogicErrorResponse()
        {
            // Arrange
            var command = this.CreateCommand();
            command.Code = "USD";

            // Act
            var response = this.HttpClient.PostAsJsonAsync($"api/currencies", command).Result;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(response.Headers.GetValues("X-Payload-Type").Single(), Is.EqualTo("Error"));

            var error = response.ReadAsError();

            Assert.That(error.Code, Is.EqualTo(typeof(CodeAlreadyExistsException).FullName));
            Assert.That(error.Message, Is.EqualTo("Code already exists."));
        }

        private CreateCurrencyCommand CreateCommand()
        {
            var command = new CreateCurrencyCommand
            {
                Name = "Wat Dynero",
                Code = "WAT",
            };

            return command;
        }
    }
}
