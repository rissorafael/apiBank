using FluentValidation;

namespace BancoChu.Application.Dtos.Accounts
{
    public class GetStatementRequestValidator : AbstractValidator<GetStatementRequestDto>
    {
        public GetStatementRequestValidator()
        {
            RuleFor(x => x.AccountId)
                .NotEmpty();

            RuleFor(x => x.StartDate)
                .NotEmpty();

            RuleFor(x => x.EndDate)
                .NotEmpty();

            RuleFor(x => x)
                .Must(x => x.StartDate <= x.EndDate)
                .WithMessage("A data inicial não pode ser maior que a data final.");
        }
    }
}