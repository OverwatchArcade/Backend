using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Discord;
using OverwatchArcade.API.Factories.Interfaces;
using OverwatchArcade.API.Services.AuthService;
using OverwatchArcade.API.Utility;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Persistence;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Services
{
    public class AuthServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IDiscordApiClient> _discordApiClientMock;
        private readonly Mock<IAuthenticationToken> _authenticationTokenMock;
        private readonly Mock<IValidator<DiscordLoginDto>> _registerValidatorMock;
        private readonly Mock<IServiceResponseFactory<string>> _serviceResponseFactoryMock;
        
        private readonly AuthService _authService;
        private const string DiscordBearerToken = "12345";
        private const string DiscordRedirectUri = "owarcade.local/redirect";
        private const string ExpectedJwtToken = "99999";

        public AuthServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _authenticationTokenMock = new Mock<IAuthenticationToken>();
            _discordApiClientMock = new Mock<IDiscordApiClient>();
            _authenticationTokenMock = new Mock<IAuthenticationToken>();
            _registerValidatorMock = new Mock<IValidator<DiscordLoginDto>>();
            _serviceResponseFactoryMock = new Mock<IServiceResponseFactory<string>>();

            _authService = new AuthService(_unitOfWorkMock.Object, _discordApiClientMock.Object, _authenticationTokenMock.Object, _registerValidatorMock.Object, _serviceResponseFactoryMock.Object);
        }

        [Fact]
        public void Constructor()
        {
            var constructor = new AuthService(_unitOfWorkMock.Object, _discordApiClientMock.Object, _authenticationTokenMock.Object, _registerValidatorMock.Object, _serviceResponseFactoryMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new AuthService(
                null, _discordApiClientMock.Object, _authenticationTokenMock.Object, _registerValidatorMock.Object, _serviceResponseFactoryMock.Object
            ));
            
            Should.Throw<ArgumentNullException>(() => new AuthService(
                _unitOfWorkMock.Object, null, _authenticationTokenMock.Object, _registerValidatorMock.Object, _serviceResponseFactoryMock.Object
            ));
            
            Should.Throw<ArgumentNullException>(() => new AuthService(
                _unitOfWorkMock.Object, _discordApiClientMock.Object, null, _registerValidatorMock.Object, _serviceResponseFactoryMock.Object
            ));
            
            Should.Throw<ArgumentNullException>(() => new AuthService(
                _unitOfWorkMock.Object, _discordApiClientMock.Object, _authenticationTokenMock.Object, null, _serviceResponseFactoryMock.Object
            ));
            
            Should.Throw<ArgumentNullException>(() => new AuthService(
                _unitOfWorkMock.Object, _discordApiClientMock.Object, _authenticationTokenMock.Object, _registerValidatorMock.Object, null
            ));
        }

        [Fact]
        public async Task RegisterAndLogin_Success_RegistersContributor()
        {
            // Arrange
            var discordToken = new DiscordToken { AccessToken = "123"};
            var discordLoginDto = new DiscordLoginDto { Username = "System", Email = "system@oarcade.local"};
            var serviceResponse = new ServiceResponse<string>();
            var validateResultMock = new Mock<ValidationResult>();
            Contributor createdContributor = null;
            
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            _discordApiClientMock.Setup(x => x.GetDiscordToken(DiscordBearerToken, DiscordRedirectUri)).ReturnsAsync(discordToken);
            _discordApiClientMock.Setup(x => x.MakeDiscordOAuthCall(discordToken.AccessToken)).ReturnsAsync(discordLoginDto);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.FirstOrDefault(c => c.Email.Equals(discordLoginDto.Email))).Returns((Contributor)null);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Add(It.IsAny<Contributor>())).Callback<Contributor>(c => { createdContributor = c;});
            _registerValidatorMock.Setup(x => x.ValidateAsync(discordLoginDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);
            _authenticationTokenMock.Setup(x => x.CreateJwtToken(It.IsAny<Contributor>())).Returns(ExpectedJwtToken);
            _serviceResponseFactoryMock.Setup(x => x.Create(It.IsAny<string>())).Returns(serviceResponse);

            // Act
            var result = await _authService.RegisterAndLogin(DiscordBearerToken, DiscordRedirectUri);
            
            // Assert
            result.Success.ShouldBeTrue();
            createdContributor.ShouldNotBeNull();
            createdContributor.Username.ShouldBe(discordLoginDto.Username);
            createdContributor.Email.ShouldBe(discordLoginDto.Email);
            _serviceResponseFactoryMock.Verify(x => x.Create(ExpectedJwtToken), Times.Once);
            _unitOfWorkMock.Verify(x => x.ContributorRepository.Add(It.IsAny<Contributor>()), Times.Once());
            _unitOfWorkMock.Verify(x => x.Save(), Times.Once());
        }

        [Fact]
        public async Task RegisterAndLogin_Success_LoginsExistingContributor()
        {
            // Arrange
            var discordToken = new DiscordToken(){ AccessToken = "123"};
            var discordLoginDto = new DiscordLoginDto() { Username = "System", Email = "system@oarcade.local"};
            var serviceResponse = new ServiceResponse<string>();
            var existingContributor = new Contributor() { Username = "System" };
            var validateResultMock = new Mock<ValidationResult>();
            
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            _discordApiClientMock.Setup(x => x.GetDiscordToken(DiscordBearerToken, DiscordRedirectUri)).ReturnsAsync(discordToken);
            _discordApiClientMock.Setup(x => x.MakeDiscordOAuthCall(discordToken.AccessToken)).ReturnsAsync(discordLoginDto);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.FirstOrDefault(c => c.Email.Equals(discordLoginDto.Email))).Returns(existingContributor);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Add(It.IsAny<Contributor>()));
            _registerValidatorMock.Setup(x => x.ValidateAsync(discordLoginDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);
            _authenticationTokenMock.Setup(x => x.CreateJwtToken(It.IsAny<Contributor>())).Returns(ExpectedJwtToken);
            _serviceResponseFactoryMock.Setup(x => x.Create(It.IsAny<string>())).Returns(serviceResponse);

            // Act
            var result = await _authService.RegisterAndLogin(DiscordBearerToken, DiscordRedirectUri);
            
            // Assert
            result.Success.ShouldBeTrue();
            
            _serviceResponseFactoryMock.Verify(x => x.Create(ExpectedJwtToken), Times.Once);
            _unitOfWorkMock.Verify(x => x.ContributorRepository.Add(It.IsAny<Contributor>()), Times.Never());
            _unitOfWorkMock.Verify(x => x.Save(), Times.Never());
        }

        [Fact]
        public async Task RegisterAndLogin_Error_GetDiscordToken()
        {
            // Arrange
            var returnedServiceResponse = new ServiceResponse<string>{Success = false};
            _discordApiClientMock.Setup(x => x.GetDiscordToken(DiscordBearerToken, DiscordRedirectUri)).ReturnsAsync((DiscordToken) null);
            _serviceResponseFactoryMock.Setup(x => x.Error(500, "Couldn't get Discord Token")).Returns(returnedServiceResponse);

            // Act
            var result = await _authService.RegisterAndLogin(DiscordBearerToken, DiscordRedirectUri);
            
            // Assert
            result.Success.ShouldBeFalse();
            _serviceResponseFactoryMock.Verify(x => x.Error(500, "Couldn't get Discord Token"), Times.Once);
        }
        
        [Fact]
        public async Task RegisterAndLogin_Error_GetDiscordOAuthToken()
        {
            // Arrange
            var returnedServiceResponse = new ServiceResponse<string>{ Success = false};
            var discordToken = new DiscordToken { AccessToken = "123"};
            _discordApiClientMock.Setup(x => x.GetDiscordToken(DiscordBearerToken, DiscordRedirectUri)).ReturnsAsync(discordToken);
            _discordApiClientMock.Setup(x => x.MakeDiscordOAuthCall(discordToken.AccessToken)).ReturnsAsync((DiscordLoginDto) null);
            _serviceResponseFactoryMock.Setup(x => x.Error(500, "Couldn't get Discord OAuth Token")).Returns(returnedServiceResponse);

            // Act
            var result = await _authService.RegisterAndLogin(DiscordBearerToken, DiscordRedirectUri);
            
            // Assert
            result.Success.ShouldBeFalse();
            _serviceResponseFactoryMock.Verify(x => x.Error(500, "Couldn't get Discord OAuth Token"), Times.Once);
        }
        
        [Fact]
        public async Task RegisterAndLogin_Error_RegistersFailsValidation()
        {
            // Arrange
            var discordToken = new DiscordToken { AccessToken = "123"};
            var discordLoginDto = new DiscordLoginDto { Username = "System", Email = "system@oarcade.local"};
            var serviceResponse = new ServiceResponse<string> { Success = false};
            var validateResultMock = new Mock<ValidationResult>();

            validateResultMock.Setup(x => x.IsValid).Returns(false);
            _discordApiClientMock.Setup(x => x.GetDiscordToken(DiscordBearerToken, DiscordRedirectUri)).ReturnsAsync(discordToken);
            _discordApiClientMock.Setup(x => x.MakeDiscordOAuthCall(discordToken.AccessToken)).ReturnsAsync(discordLoginDto);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.FirstOrDefault(c => c.Email.Equals(discordLoginDto.Email))).Returns((Contributor)null);
            _registerValidatorMock.Setup(x => x.ValidateAsync(discordLoginDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);
            _authenticationTokenMock.Setup(x => x.CreateJwtToken(It.IsAny<Contributor>())).Returns(ExpectedJwtToken);
            _serviceResponseFactoryMock.Setup(x => x.Error(500, It.IsAny<string>())).Returns(serviceResponse);

            // Act
            var result = await _authService.RegisterAndLogin(DiscordBearerToken, DiscordRedirectUri);
            
            // Assert
            result.Success.ShouldBeFalse();
            _serviceResponseFactoryMock.Verify(x => x.Error(500, ""), Times.Once);
        }
    }
}