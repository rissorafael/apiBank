
using FluentValidation;

namespace BancoChu.Application.Dtos.Accounts
{
    public class CreateAccountsRequestValidator : AbstractValidator<CreateAccountsRequestDto>
    {
        public CreateAccountsRequestValidator()
        {
            RuleFor(x => x.AccountNumber)
              .NotEmpty().WithMessage("O número da conta é obrigatório.")
              .Length(5, 20).WithMessage("O número da conta deve ter entre 5 e 20 caracteres.")
              .Matches(@"^\d+$").WithMessage("O número da conta deve conter apenas números.");

            RuleFor(x => x.Agency)
                .NotEmpty().WithMessage("A agência é obrigatória.")
                .Length(3, 10).WithMessage("A agência deve ter entre 3 e 10 caracteres.")
                .Matches(@"^\d+$").WithMessage("A agência deve conter apenas números.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("O usuário é obrigatório.");

            RuleFor(x => x.Balance)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O saldo inicial não pode ser negativo.");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("O tipo de conta informado é inválido.");
        }
    }
}
