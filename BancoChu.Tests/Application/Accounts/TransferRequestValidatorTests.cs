using BancoChu.Application.Dtos.Accounts;
using FluentValidation.TestHelper;

namespace BancoChu.Tests.Application.Accounts
{
    public class TransferRequestValidatorTests
    {
        private readonly TransferRequestValidator _validator;

        public TransferRequestValidatorTests()
        {
            _validator = new TransferRequestValidator();
        }

        [Fact]
        public void Deve_Passar_Quando_Dados_Forem_Validos()
        {
            // Arrange
            var dto = new TransferRequestDto
            {
                DestinationAccountId = Guid.NewGuid(),
                Amount = 100m
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Deve_Erro_Quando_Valor_For_Zero()
        {
            var dto = new TransferRequestDto
            {
                DestinationAccountId = Guid.NewGuid(),
                Amount = 0
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                  .WithErrorMessage("O valor da transferência deve ser maior que zero.");
        }

        [Fact]
        public void Deve_Erro_Quando_Valor_For_Negativo()
        {
            var dto = new TransferRequestDto
            {
                DestinationAccountId = Guid.NewGuid(),
                Amount = -10
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Amount)
                  .WithErrorMessage("O valor da transferência deve ser maior que zero.");
        }

        [Fact]
        public void Deve_Erro_Quando_Conta_Destino_For_Vazia()
        {
            var dto = new TransferRequestDto
            {
                DestinationAccountId = Guid.Empty,
                Amount = 100
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.DestinationAccountId)
                  .WithErrorMessage("Conta de destino é obrigatória.");
        }



    }
}
