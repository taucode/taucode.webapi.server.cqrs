using System.Collections.Generic;
using FluentValidation;
using TauCode.Validation;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.UpdateCurrency
{
    public class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>, IParameterValidator
    {
        public UpdateCurrencyCommandValidator()
        {
            this.RuleFor(x => this.GetCurrencyId())
                .NotNull()
                .WithName("Id");

            this.RuleFor(x => x.Code)
                .NotEmpty()
                .DependentRules(() =>
                {
                    this.RuleFor(x => x.Code)
                        .CurrencyCode();
                });

            this.RuleFor(x => x.Name)
                .NotEmpty()
                .DependentRules(() =>
                {
                    this.RuleFor(x => x.Name)
                        .FullName(1, 50);
                });
        }

        private CurrencyId GetCurrencyId()
        {
            return this.Parameters.GetOrDefault("id") as CurrencyId;
        }

        public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }
}
