using BancoChu.Application.Dtos.Accounts;
using BancoChu.Domain.Enums;
using FluentValidation.TestHelper;

namespace BancoChu.Tests.Application.Accounts
{
    public class CreateAccountsRequestValidatorTests
    {
        private readonly CreateAccountsRequestValidator _validator;

        public CreateAccountsRequestValidatorTests()
        {
            _validator = new CreateAccountsRequestValidator();
        }

        [Fact]
        public void Deve_Passar_Quando_Dados_Forem_Validos()
        {
            // Arrange
            var dto = new CreateAccountsRequestDto
            {
                AccountNumber = "123456",
                Agency = "0001",
                UserId = Guid.NewGuid(),
                Balance = 1000m,
                Type = AccountType.Checking
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Deve_Erro_Quando_AccountNumber_For_Vazio()
        {
            var dto = new CreateAccountsRequestDto
            {
                AccountNumber = "",
                Agency = "0001",
                UserId = Guid.NewGuid(),
                Balance = 0,
                Type = AccountType.Checking
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.AccountNumber)
                  .WithErrorMessage("O número da conta é obrigatório.");
        }

        [Fact]
        public void Deve_Erro_Quando_AccountNumber_Tiver_Letras()
        {
            var dto = new CreateAccountsRequestDto
            {
                AccountNumber = "12AB34",
                Agency = "0001",
                UserId = Guid.NewGuid(),
                Balance = 0,
                Type = AccountType.Checking
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.AccountNumber)
                  .WithErrorMessage("O número da conta deve conter apenas números.");
        }

        [Fact]
        public void Deve_Erro_Quando_Agency_For_Menor_Que_3_Caracteres()
        {
            var dto = new CreateAccountsRequestDto
            {
                AccountNumber = "123456",
                Agency = "01",
                UserId = Guid.NewGuid(),
                Balance = 0,
                Type = AccountType.Checking
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Agency)
                  .WithErrorMessage("A agência deve ter entre 3 e 10 caracteres.");
        }

        [Fact]
        public void Deve_Erro_Quando_UserId_For_Empty()
        {
            var dto = new CreateAccountsRequestDto
            {
                AccountNumber = "123456",
                Agency = "0001",
                UserId = Guid.Empty,
                Balance = 0,
                Type = AccountType.Checking
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.UserId)
                  .WithErrorMessage("O usuário é obrigatório.");
        }

        [Fact]
        public void Deve_Erro_Quando_Balance_For_Negativo()
        {
            var dto = new CreateAccountsRequestDto
            {
                AccountNumber = "123456",
                Agency = "0001",
                UserId = Guid.NewGuid(),
                Balance = -1,
                Type = AccountType.Checking
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Balance)
                  .WithErrorMessage("O saldo inicial não pode ser negativo.");
        }

        [Fact]
        public void Deve_Erro_Quando_Tipo_For_Invalido()
        {
            var dto = new CreateAccountsRequestDto
            {
                AccountNumber = "123456",
                Agency = "0001",
                UserId = Guid.NewGuid(),
                Balance = 0,
                Type = (AccountType)999
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Type)
                  .WithErrorMessage("O tipo de conta informado é inválido.");
        }


    }
}
