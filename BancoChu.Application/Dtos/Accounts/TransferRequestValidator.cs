using FluentValidation;

namespace BancoChu.Application.Dtos.Accounts
{
    public class TransferRequestValidator : AbstractValidator<TransferRequestDto>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("O valor da transferência deve ser maior que zero.");

            RuleFor(x => x.DestinationAccountId)
                .NotEmpty()
                .WithMessage("Conta de destino é obrigatória.");
        }
    }
}