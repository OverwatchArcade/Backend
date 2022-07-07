﻿using System;
using System.Linq.Expressions;
using Moq;
using OverwatchArcade.API.Dtos.Discord;
using OverwatchArcade.API.Validators;
using OverwatchArcade.Persistence;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Validators
{
    public class LoginValidatorTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly LoginValidator _loginValidator;

        public LoginValidatorTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _loginValidator = new LoginValidator(_unitOfWorkMock.Object);
        }
        
        [Fact]
        public void Constructor()
        {
            var constructor = new LoginValidator(_unitOfWorkMock.Object);
            Assert.NotNull(constructor);
        }
        
        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new LoginValidator(
                null
            ));
        }

        [Fact]
        public void LoginValidator_Success()
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
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(It.IsAny<Expression<Func<Domain.Models.Contributor, bool>>>())).Returns(true);
            
            // Act
            var result = _loginValidator.Validate(login);

            // Assert
            Assert.True(result.IsValid);
        }
        
        [Fact]
        public void LoginValidator_Invalid_UserDoesNotExist()
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
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(It.IsAny<Expression<Func<Domain.Models.Contributor, bool>>>())).Returns(false);
            
            // Act
            var result = new LoginValidator(_unitOfWorkMock.Object).Validate(login);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Incorrect email and/or password", result.Errors[0].ErrorMessage);
        }
    }
}