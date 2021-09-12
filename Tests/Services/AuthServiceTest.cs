using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Dtos.Discord;
using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence;
using OWArcadeBackend.Persistence.Repositories.Interfaces;
using OWArcadeBackend.Services.AuthService;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Services
{
    public class AuthServiceTest
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<AuthService>> _loggerMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<IAuthRepository> _authRepositoryMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;

        public AuthServiceTest()
        {
            _configurationMock = new Mock<IConfiguration>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _authRepositoryMock = new Mock<IAuthRepository>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new AuthService(_configurationMock.Object, _mapperMock.Object, _unitOfWorkMock.Object, _loggerMock.Object, _authRepositoryMock.Object, _webHostEnvironmentMock.Object, _httpClient);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new AuthService(
                null,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _webHostEnvironmentMock.Object,
                _httpClient
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                null,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _webHostEnvironmentMock.Object,
                _httpClient
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _mapperMock.Object,
                null,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _webHostEnvironmentMock.Object,
                _httpClient
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                null,
                _authRepositoryMock.Object,
                _webHostEnvironmentMock.Object,
                _httpClient
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                null,
                _webHostEnvironmentMock.Object,
                _httpClient
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                null,
                _httpClient
            ));
            
            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _webHostEnvironmentMock.Object,
                null
            ));
        }

        [Fact]
        public async Task TestRegisterAndLogin_LoginAccount()
        {
            // arrange
            const string discordToken = "12345";
            
            var discordClientIdConfiguration = new Mock<IConfigurationSection>();
            var discordClientSecretConfiguration = new Mock<IConfigurationSection>();
            var discordRedirectUriConfiguration = new Mock<IConfigurationSection>();
            var jwtTokenValue = new Mock<IConfigurationSection>();
            var discordResponseToken = new Token()
            {
                AccessToken = "6qrZcUqja7812RVdnEKjpzOL4CvHBFG",
                TokenType = "Bearer",
                ExpiresIn = 604800,
                RefreshToken = "D43f5y0ahjqew82jZ4NViEr2YafMKhue",
                Scope = "Identify"
            };
            var tokenResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(discordResponseToken))
            };

            var contributor = new Contributor()
            {
                Id = new Guid("12D20088-719F-48A3-859D-A255CDFD1273"),
                Email = "info@overwatcharcade.today",
                Username = "System",
                Group = ContributorGroup.Admin,
                Avatar = "image.jpg"
            };
            
            var discordLoginDto = new DiscordLoginDto()
            {
                Id = contributor.Id.ToString(),
                Email = contributor.Email,
                Username = contributor.Username,
                Avatar = contributor.Avatar
            };
            
            var loginResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(discordLoginDto))
            };

            discordClientIdConfiguration.Setup(x => x.Value).Returns("111");
            discordClientSecretConfiguration.Setup(x => x.Value).Returns("222");
            discordRedirectUriConfiguration.Setup(x => x.Value).Returns("https://overwatcharcade.today");
            jwtTokenValue.Setup(x => x.Value).Returns("cpHUeKXCtXKO25UWV5p8cpHUeKXCtXKO2");
            

            _configurationMock.Setup(x => x.GetSection("Discord:clientId")).Returns(discordClientIdConfiguration.Object);
            _configurationMock.Setup(x => x.GetSection("Discord:clientSecret")).Returns(discordClientSecretConfiguration.Object);
            _configurationMock.Setup(x => x.GetSection("Discord:redirectUri")).Returns(discordRedirectUriConfiguration.Object);
            _configurationMock.Setup(x => x.GetSection("Jwt:Token")).Returns(jwtTokenValue.Object);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(y => y.Email.Equals(discordLoginDto.Email))).Returns(true);
            _unitOfWorkMock.Setup(x => x.WhitelistRepository.IsDiscordWhitelisted(discordLoginDto.Id)).Returns(true);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.SingleOrDefault(It.IsAny<Expression<Func<Contributor,bool>>>())).Returns(contributor);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(It.IsAny<Expression<Func<Contributor,bool>>>())).Returns(true);
            

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.ToString().StartsWith("https://discord.com/api/oauth2/token")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(tokenResponse);
            
            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.ToString().StartsWith("https://discord.com/api/users/@me")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(loginResponse);
            
            // act
            var result = await new AuthService(
                _configurationMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _webHostEnvironmentMock.Object,
                _httpClient
            ).RegisterAndLogin(discordToken);

            // assert
            Assert.True(result.Success);
        }
        
        [Fact]
        public async Task TestRegisterAndLogin_RegisterAccount()
        {
            // arrange
            const string discordToken = "12345";
            
            var discordClientIdConfiguration = new Mock<IConfigurationSection>();
            var discordClientSecretConfiguration = new Mock<IConfigurationSection>();
            var discordRedirectUriConfiguration = new Mock<IConfigurationSection>();
            var jwtTokenValue = new Mock<IConfigurationSection>();
            var discordResponseToken = new Token()
            {
                AccessToken = "6qrZcUqja7812RVdnEKjpzOL4CvHBFG",
                TokenType = "Bearer",
                ExpiresIn = 604800,
                RefreshToken = "D43f5y0ahjqew82jZ4NViEr2YafMKhue",
                Scope = "Identify"
            };
            var tokenResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(discordResponseToken))
            };

            Contributor newContributor = null;
            var expectedContributor = new Contributor()
            {
                Id = new Guid("1ECC7B44-76F6-401C-8E64-3B53008F5957"),
                Email = "info@overwatcharcade.today",
                Username = "System",
                Group = ContributorGroup.Contributor,
            };
            
            var discordLoginDto = new DiscordLoginDto()
            {
                Id = expectedContributor.Id.ToString(),
                Email = expectedContributor.Email,
                Username = expectedContributor.Username,
                Avatar = expectedContributor.Avatar
            };
            
            var loginResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(discordLoginDto))
            };

            discordClientIdConfiguration.Setup(x => x.Value).Returns("111");
            discordClientSecretConfiguration.Setup(x => x.Value).Returns("222");
            discordRedirectUriConfiguration.Setup(x => x.Value).Returns("https://overwatcharcade.today");
            jwtTokenValue.Setup(x => x.Value).Returns("cpHUeKXCtXKO25UWV5p8cpHUeKXCtXKO2");
            

            _configurationMock.Setup(x => x.GetSection("Discord:clientId")).Returns(discordClientIdConfiguration.Object);
            _configurationMock.Setup(x => x.GetSection("Discord:clientSecret")).Returns(discordClientSecretConfiguration.Object);
            _configurationMock.Setup(x => x.GetSection("Discord:redirectUri")).Returns(discordRedirectUriConfiguration.Object);
            _configurationMock.Setup(x => x.GetSection("Jwt:Token")).Returns(jwtTokenValue.Object);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(y => y.Email.Equals(discordLoginDto.Email))).Returns(true);
            _unitOfWorkMock.Setup(x => x.WhitelistRepository.IsDiscordWhitelisted(discordLoginDto.Id)).Returns(true);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(It.IsAny<Expression<Func<Contributor,bool>>>())).Returns(false);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.GetBy(It.IsAny<Expression<Func<Contributor,bool>>>())).Returns(expectedContributor);
            _authRepositoryMock.Setup(x => x.Add(It.IsAny<Contributor>())).Callback<Contributor>((entity) => newContributor = entity);
            

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.ToString().StartsWith("https://discord.com/api/oauth2/token")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(tokenResponse);
            
            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.ToString().StartsWith("https://discord.com/api/users/@me")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(loginResponse);
            
            // act
            var result = await new AuthService(
                _configurationMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _webHostEnvironmentMock.Object,
                _httpClient
            ).RegisterAndLogin(discordToken);

            // assert
            Assert.True(result.Success);
            expectedContributor.Username.ShouldBe(newContributor.Username);
            expectedContributor.Email.ShouldBe(newContributor.Email);
            expectedContributor.Group.ShouldBe(newContributor.Group);
        }

        // [Fact]
        // public async Task TestRegisterAndLogin_UploadAvatar()
        // {
        //     // arrange
        //     var avatarMock = new Mock<IFormFile>();
        //     avatarMock.SetupGet(x => x.Length).Returns(750000);
        //     avatarMock.SetupGet(x => x.ContentType).Returns("image/jpeg");
        //     var contributorAvatarDto = new ContributorAvatarDto()
        //     {
        //         Avatar = avatarMock.Object
        //     };
        //     var contributor = new Contributor()
        //     {
        //         Id = new Guid("12D20088-719F-48A3-859D-A255CDFD1273"),
        //         Email = "info@overwatcharcade.today",
        //         Username = "System",
        //         Group = ContributorGroup.Admin,
        //         Avatar = "default.jpg"
        //     };
        //
        //     _webHostEnvironmentMock.SetupGet(x => x.WebRootPath).Returns(Directory.GetCurrentDirectory);
        //     _unitOfWorkMock.Setup(x => x.ContributorRepository.Find(It.IsAny<Expression<Func<Contributor,bool>>>())).Returns(new List<Contributor> {contributor});
        //     
        //     // act
        //     var result = await new AuthService(
        //         _configurationMock.Object,
        //         _mapperMock.Object,
        //         _unitOfWorkMock.Object,
        //         _loggerMock.Object,
        //         _authRepositoryMock.Object,
        //         _webHostEnvironmentMock.Object,
        //         _httpClient
        //     ).UploadAvatar(contributorAvatarDto, contributor.Id);
        //     
        //     // assert
        //     _unitOfWorkMock.Verify(x => x.Save());
        // }
    }
}