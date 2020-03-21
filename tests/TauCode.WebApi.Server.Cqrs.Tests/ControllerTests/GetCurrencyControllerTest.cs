using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TauCode.WebApi.Server.Cqrs.Tests.ControllerTests
{
    [TestFixture]
    public class GetCurrencyControllerTest : AppHostTest
    {
        [Test]
        [TestCase("code", "USD", Description = "Get by code")]
        [TestCase("id", "ce49c7a1-66f8-494e-b5cd-9b9b925637ee", Description = "Get by id")]
        public void GetCurrency_ValidRequest_ReturnsOkResultAndCurrency(string paramName, string value)
        {
            // Arrange
            var url = $"api/currencies/by-prop?{paramName}={value}";

            // Act
            var response = this.HttpClient.GetAsync(url).Result;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var currency = response.ReadAs<GetCurrencyQueryResult>();

            Assert.That(currency.Id, Is.EqualTo(IntegrationTestHelper.UsdId));
            Assert.That(currency.Code, Is.EqualTo("USD"));
            Assert.That(currency.Name, Is.EqualTo("United States dollar"));
        }

        [Test]
        [TestCase("ce49c7a1-66f8-494e-b5cd-9b9b925637ee", "USD", Description = "Both code and id provided")]
        [TestCase(null, null, Description = "None of code and id provided")]
        public void GetCurrency_BadRequest_ReturnsBadRequestResult(string id, string code)
        {
            // Arrange
            var parameterDictionary = new Dictionary<string, string>
            {
                { "id", id },
                { "code", code },
            };

            var sb = new StringBuilder("api/currencies/by-prop");
            var queryString = parameterDictionary.BuildQueryString();
            if (queryString.Length > 0)
            {
                sb.Append("?");
                sb.Append(queryString);
            }

            var url = sb.ToString();

            // Act
            var response = this.HttpClient.GetAsync(url).Result;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var validationErrorResponse = response.ReadAsValidationError();

            validationErrorResponse
                .ShouldHaveFailureNumber(1)
                .ShouldContainFailure("query", "CodedEntityQueryValidator", "Either 'Id' or 'Code' must be not null.");
        }

        [Test]
        [TestCase("code", "WAT", Description = "Get by non-existing code")]
        [TestCase("id", "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee", Description = "Get by non-existing id")]
        public void GetCurrency_NonExistingId_ReturnsNotFoundResult(string paramName, string value)
        {
            // Arrange
            var url = $"api/currencies/by-prop?{paramName}={value}";

            // Act
            var response = this.HttpClient.GetAsync(url).Result;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            var error = response.ReadAsError();
            Assert.That(error.Code, Is.EqualTo(typeof(CurrencyNotFoundException).FullName));
            Assert.That(error.Message, Is.EqualTo("Currency not found."));
        }
    }
}
