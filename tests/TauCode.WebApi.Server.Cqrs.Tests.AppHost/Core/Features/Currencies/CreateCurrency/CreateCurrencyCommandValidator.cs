using FluentValidation;
using TauCode.Validation;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.Core.Features.Currencies.CreateCurrency
{
    public class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
    {
        public CreateCurrencyCommandValidator()
        {
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
    }
}
