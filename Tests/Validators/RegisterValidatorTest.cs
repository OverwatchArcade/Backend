using System;
using System.Linq.Expressions;
using Moq;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence;
using OWArcadeBackend.Validators;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Validators
{
    public class RegisterValidatorTest
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;

        public RegisterValidatorTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }
        
        [Fact]
        public void TestConstructor()
        {
            var constructor = new RegisterValidator(_unitOfWorkMock.Object);
            Assert.NotNull(constructor);
        }
        
        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new RegisterValidator(
                null
            ));
        }

        [Fact]
        public void TestRegisterValidator_Success()
        {
            // Arrange
            var discordLoginDto = new DiscordLoginDto()
            {
                Id = "12345",
                Username = "System",
                Email = "system@overwatcharcade.today",
                Avatar = "avatar.jpg",
                Verified = true
            };
            
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(It.IsAny<Expression<Func<Models.Contributor, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.WhitelistRepository.Exists(It.IsAny<Expression<Func<Whitelist, bool>>>())).Returns(false);
            
            // Act
            var result = new RegisterValidator(_unitOfWorkMock.Object).Validate(discordLoginDto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(3, result.Errors.Count);
            Assert.Equal("Username is already taken", result.Errors[0].ErrorMessage);
            Assert.Equal("Email is already registered", result.Errors[1].ErrorMessage);
            Assert.Equal($"User id {discordLoginDto.Id} is not whitelisted", result.Errors[2].ErrorMessage);
        }
    }
}