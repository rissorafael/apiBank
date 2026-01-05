using BancoChu.Application.Dtos.Accounts;
using FluentValidation.TestHelper;

namespace BancoChu.Tests.Application.Accounts
{
    public class GetStatementRequestValidatorTests
    {
        private readonly GetStatementRequestValidator _validator;

        public GetStatementRequestValidatorTests()
        {
            _validator = new GetStatementRequestValidator();
        }

        [Fact]
        public void Deve_retornar_erro_quando_AccountId_for_vazio()
        {
            // Arrange
            var dto = new GetStatementRequestDto
            {
                AccountId = Guid.Empty,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AccountId);
        }

        [Fact]
        public void Deve_retornar_erro_quando_StartDate_for_maior_que_EndDate()
        {
            // Arrange
            var dto = new GetStatementRequestDto
            {
                AccountId = Guid.NewGuid(),
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x)
                  .WithErrorMessage("A data inicial não pode ser maior que a data final.");
        }

        [Fact]
        public void Nao_deve_retornar_erro_quando_datas_forem_validas()
        {
            // Arrange
            var dto = new GetStatementRequestDto
            {
                AccountId = Guid.NewGuid(),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}