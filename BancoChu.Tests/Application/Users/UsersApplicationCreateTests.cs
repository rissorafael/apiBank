using BancoChu.Application;
using BancoChu.Application.Dtos.Users;
using BancoChu.Domain.Entities;
using BancoChu.Domain.Interfaces;
using FluentAssertions;
using Moq;


namespace BancoChu.Tests.Application.Users
{
    public class UsersApplicationCreateTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UsersApplication _usersApplication;

        public UsersApplicationCreateTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _usersApplication = new UsersApplication(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Deve_Criar_Usuario_Com_Dados_Corretos()
        {
            // Arrange
            var request = new CreateUserRequestDto
            {
                Email = "teste@bancochu.com",
                Password = "Teste@123456"
            };

            User? userSaved = null;

            _userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(user => userSaved = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _usersApplication.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);
            result.Email.Should().Be(request.Email);
            result.Status.Should().Be(1);

            userSaved.Should().NotBeNull();
            userSaved!.Email.Should().Be(request.Email);
            userSaved.Password.Should().NotBe(request.Password);
            BCrypt.Net.BCrypt.Verify(request.Password, userSaved.Password)
                .Should().BeTrue();

            _userRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<User>()),
                Times.Once
            );
        }
    }
}
