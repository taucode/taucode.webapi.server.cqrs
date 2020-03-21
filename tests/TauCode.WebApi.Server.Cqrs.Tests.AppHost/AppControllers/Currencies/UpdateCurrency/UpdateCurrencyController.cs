using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        [SwaggerOperation("Update currency", "Updates a currency.", Tags = new[] {"Currencies"})]
        [SwaggerResponse(
            StatusCodes.Status200OK, 
            "Currency was updated and returned to user.",
            Type = typeof(GetCurrencyQueryResult))]
        [SwaggerResponse(
            StatusCodes.Status400BadRequest,
            "Bad data for currency updating.",
            Type = typeof(ValidationErrorDto))]
        [SwaggerResponse(
            StatusCodes.Status404NotFound,
            "Currency not found.",
            Type = typeof(ErrorDto))]
        [Route("api/currencies/{id}")]
        [HttpPut]
        public IActionResult UpdateCurrency([FromRoute] CurrencyId id, UpdateCurrencyCommand command)
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
                var url = this.Url.Action("GetCurrency", "GetCurrency", new {id});

                return this.Ok(queryResult);
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
