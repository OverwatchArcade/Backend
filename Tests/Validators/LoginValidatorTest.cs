using System;
using System.Linq.Expressions;
using Moq;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Dtos.Discord;
using OWArcadeBackend.Persistence;
using OWArcadeBackend.Validators;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Validators
{
    public class LoginValidatorTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public LoginValidatorTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }
        
        [Fact]
        public void TestConstructor()
        {
            var constructor = new LoginValidator(_unitOfWorkMock.Object);
            Assert.NotNull(constructor);
        }
        
        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new LoginValidator(
                null
            ));
        }

        [Fact]
        public void TestLoginValidator_Success()
        {
            // Arrange
            var login = new DiscordLoginDto()
            {
                Id = "12345",
                Username = "System",
                Email = "system@overwatcharcade.today",
                Avatar = "avatar.jpg",
                Verified = true
            };
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(It.IsAny<Expression<Func<Models.Contributor, bool>>>())).Returns(true);
            
            // Act
            var result = new LoginValidator(_unitOfWorkMock.Object).Validate(login);

            // Assert
            Assert.True(result.IsValid);
        }
        
        [Fact]
        public void TestLoginValidator_Invalid_UserDoesNotExist()
        {
            // Arrange
            var login = new DiscordLoginDto()
            {
                Id = "456789",
                Username = "System",
                Email = "system@overwatcharcade.today",
                Avatar = "avatar.jpg",
                Verified = true
            };
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(It.IsAny<Expression<Func<Models.Contributor, bool>>>())).Returns(false);
            
            // Act
            var result = new LoginValidator(_unitOfWorkMock.Object).Validate(login);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Incorrect email and/or password", result.Errors[0].ErrorMessage);
        }
    }
}