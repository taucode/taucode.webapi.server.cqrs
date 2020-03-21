using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TauCode.Cqrs.Commands;
using TauCode.Cqrs.Queries;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.GetCurrency;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.UpdateCurrency;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies.Exceptions;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.AppControllers.Currencies.UpdateCurrency
{
    [ApiController]
    public class UpdateCurrencyController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryRunner _queryRunner;

        public UpdateCurrencyController(ICommandDispatcher commandDispatcher, IQueryRunner queryRunner)
        {
            _commandDispatcher = commandDispatcher;
            _queryRunner = queryRunner;
        }

        [SwaggerOperation(Tags = new[] { "Currencies" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Currency was updated and returned to user")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad data for currency updating", typeof(ValidationErrorDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Currency not found")]
        [Route("api/currencies/{id}", Name = "UpdateCurrency")]
        [HttpPut]
        public IActionResult UpdateCurrency([FromRoute]CurrencyId id, UpdateCurrencyCommand command)
        {
            command.Id = id;

            try
            {
                _commandDispatcher.Dispatch(command);

                var query = new GetCurrencyQuery
                {
                    Id = id,
                };

                _queryRunner.Run(query);

                var queryResult = query.GetResult();
                var url = this.Url.Action("GetCurrency", "GetCurrency", new { id });

                return this.UpdatedOk(id.ToString(), url, queryResult);
            }
            catch (CurrencyNotFoundException ex)
            {
                return this.NotFoundError(ex);
            }
            catch (CodeAlreadyExistsException ex)
            {
                return this.ConflictError(ex);
            }
        }
    }
}
