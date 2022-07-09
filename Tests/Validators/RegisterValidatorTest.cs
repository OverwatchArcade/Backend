using System;
using System.Linq.Expressions;
using Moq;
using OverwatchArcade.API.Dtos.Discord;
using OverwatchArcade.API.Validators;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Persistence;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Validators
{
    public class RegisterValidatorTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RegisterValidator _registerValidator;

        public RegisterValidatorTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _registerValidator = new RegisterValidator(_unitOfWorkMock.Object);
        }
        
        [Fact]
        public void Constructor()
        {
            var constructor = new RegisterValidator(_unitOfWorkMock.Object);
            Assert.NotNull(constructor);
        }
        
        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new RegisterValidator(
                null
            ));
        }

        [Fact]
        public void RegisterValidator_Success()
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
            
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(It.IsAny<Expression<Func<Domain.Models.Contributor, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.WhitelistRepository.Exists(It.IsAny<Expression<Func<Whitelist, bool>>>())).Returns(false);
            
            // Act
            var result = _registerValidator.Validate(discordLoginDto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(3, result.Errors.Count);
            Assert.Equal("Username is already taken", result.Errors[0].ErrorMessage);
            Assert.Equal("Email is already registered", result.Errors[1].ErrorMessage);
            Assert.Equal($"User id {discordLoginDto.Id} is not whitelisted", result.Errors[2].ErrorMessage);
        }
    }
}