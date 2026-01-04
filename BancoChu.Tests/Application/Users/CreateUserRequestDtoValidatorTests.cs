using BancoChu.Application.Dtos.Users;
using FluentValidation.TestHelper;

namespace BancoChu.Tests.Application.Users
{
    public class CreateUserRequestDtoValidatorTests
    {
        private readonly CreateUserRequestDtoValidator _validator;

        public CreateUserRequestDtoValidatorTests()
        {
            _validator = new CreateUserRequestDtoValidator();
        }

        [Fact]
        public void Deve_Passar_Quando_Dados_Forem_Validos()
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                Email = "usuario@bancochu.com",
                Password = "Senha@123"
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Deve_Erro_Quando_Email_For_Invalido()
        {
            var dto = new CreateUserRequestDto
            {
                Email = "email-invalido",
                Password = "Senha@123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("O e-mail informado é inválido.");
        }

        [Fact]
        public void Deve_Erro_Quando_Senha_For_Menor_Que_8_Caracteres()
        {
            var dto = new CreateUserRequestDto
            {
                Email = "usuario@bancochu.com",
                Password = "Ab1@"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("A senha deve conter no mínimo 8 caracteres.");
        }

        [Fact]
        public void Deve_Erro_Quando_Senha_Nao_Tiver_Letra_Maiuscula()
        {
            var dto = new CreateUserRequestDto
            {
                Email = "usuario@bancochu.com",
                Password = "senha@123"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("A senha deve conter ao menos uma letra maiúscula.");
        }

        [Fact]
        public void Deve_Erro_Quando_Senha_Nao_Tiver_Caractere_Especial()
        {
            var dto = new CreateUserRequestDto
            {
                Email = "usuario@bancochu.com",
                Password = "Senha1234"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("A senha deve conter ao menos um caractere especial.");
        }



    }
}
