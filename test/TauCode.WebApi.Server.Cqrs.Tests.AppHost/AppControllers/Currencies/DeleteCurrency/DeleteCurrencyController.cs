using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TauCode.Cqrs.Commands;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.DeleteCurrency;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies.Exceptions;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.AppControllers.Currencies.DeleteCurrency
{
    [ApiController]
    public class DeleteCurrencyController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public DeleteCurrencyController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [SwaggerOperation("Delete currency", "Deletes a currency.", Tags = new[] { "Currencies" })]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Currency was deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Currency not found.", Type = typeof(ErrorDto))]
        [HttpDelete]
        [Route("api/currencies/{id}")]
        public IActionResult DeleteCurrency([FromRoute] CurrencyId id)
        {
            var command = new DeleteCurrencyCommand
            {
                Id = id,
            };

            try
            {
                _commandDispatcher.Dispatch(command);

                return this.DeletedNoContent(id.ToString());
            }
            catch (CurrencyNotFoundException ex)
            {
                return this.NotFoundError(ex);
            }
        }
    }
}
