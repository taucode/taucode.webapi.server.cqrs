using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TauCode.Cqrs.Commands;
using TauCode.Cqrs.Queries;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Exceptions;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.CreateCurrency;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.GetCurrency;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.AppControllers.Currencies.CreateCurrency
{
    [ApiController]
    public class CreateCurrencyController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryRunner _queryRunner;

        public CreateCurrencyController(ICommandDispatcher commandDispatcher, IQueryRunner queryRunner)
        {
            _commandDispatcher = commandDispatcher;
            _queryRunner = queryRunner;
        }

        [SwaggerOperation("Create currency", Tags = new[] { "Currencies" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Currency has been created")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad data for currency creation", typeof(ValidationErrorDto))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Failed to create currency")]
        [HttpPost]
        [Route("api/currencies", Name = "CreateCurrency")]
        public IActionResult CreateCurrency(CreateCurrencyCommand command)
        {
            try
            {
                _commandDispatcher.Dispatch(command);
            }
            catch (CodeAlreadyExistsException ex)
            {
                return this.ConflictError(ex);
            }

            var id = command.GetResult();
            var query = new GetCurrencyQuery
            {
                Id = id,
            };

            _queryRunner.Run(query);

            var queryResult = query.GetResult();
            var url = this.Url.Action("GetCurrency", "GetCurrency", new { id });

            return this.Created(url, queryResult);
        }
    }
}
