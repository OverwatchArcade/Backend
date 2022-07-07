using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using OverwatchArcade.API.Dtos.Discord;
using OverwatchArcade.API.Services.AuthService;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Persistence;
using OverwatchArcade.Persistence.Repositories.Interfaces;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Services
{
    public class AuthServiceTest
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

        public AuthServiceTest()
        {
            _configurationMock = new Mock<IConfiguration>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _authRepositoryMock = new Mock<IAuthRepository>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public void Constructor()
        {
            var constructor = new AuthService(_configurationMock.Object, _unitOfWorkMock.Object, _loggerMock.Object, _authRepositoryMock.Object, _httpClientFactoryMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new AuthService(
                null,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _httpClientFactoryMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                null,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _httpClientFactoryMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _unitOfWorkMock.Object,
                null,
                _authRepositoryMock.Object,
                _httpClientFactoryMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                null,
                _httpClientFactoryMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                null
            ));
        }

        [Fact]
        public async Task RegisterAndLogin_LoginAccount()
        {
            // arrange
            const string discordToken = "12345";
            const string discordRedirectUri = "https://site/auth/callback";

            var discordClientIdConfiguration = new Mock<IConfigurationSection>();
            var discordClientSecretConfiguration = new Mock<IConfigurationSection>();
            var discordRedirectUriConfiguration = new Mock<IConfigurationSection>();
            var jwtTokenValue = new Mock<IConfigurationSection>();
            var discordResponseToken = new DiscordToken()
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
                Group = ContributorGroup.Developer,
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
            _unitOfWorkMock.Setup(x => x.ContributorRepository.FirstOrDefault(It.IsAny<Expression<Func<Contributor, bool>>>())).Returns(contributor);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(It.IsAny<Expression<Func<Contributor, bool>>>())).Returns(true);


            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.ToString().StartsWith("https://discord.com/api/oauth2/token")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(tokenResponse);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.ToString().StartsWith("https://discord.com/api/users/@me")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(loginResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);


            // act
            var result = await new AuthService(
                _configurationMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _httpClientFactoryMock.Object
            ).RegisterAndLogin(discordToken, discordRedirectUri);

            // assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task RegisterAndLogin_RegisterAccount()
        {
            // arrange
            const string discordToken = "12345";
            const string discordRedirectUri = "https://site/auth/callback";

            var discordClientIdConfiguration = new Mock<IConfigurationSection>();
            var discordClientSecretConfiguration = new Mock<IConfigurationSection>();
            var discordRedirectUriConfiguration = new Mock<IConfigurationSection>();
            var jwtTokenValue = new Mock<IConfigurationSection>();
            var discordResponseToken = new DiscordToken()
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
            _unitOfWorkMock.Setup(x => x.ContributorRepository.Exists(It.IsAny<Expression<Func<Contributor, bool>>>())).Returns(false);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.FirstOrDefaultASync(It.IsAny<Expression<Func<Contributor, bool>>>())).ReturnsAsync(expectedContributor);
            _authRepositoryMock.Setup(x => x.Add(It.IsAny<Contributor>())).Callback<Contributor>((entity) => newContributor = entity);

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.ToString().StartsWith("https://discord.com/api/oauth2/token")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(tokenResponse);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.ToString().StartsWith("https://discord.com/api/users/@me")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(loginResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // act
            var result = await new AuthService(
                _configurationMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _httpClientFactoryMock.Object
            ).RegisterAndLogin(discordToken, discordRedirectUri);

            // assert
            Assert.True(result.Success);
            expectedContributor.Username.ShouldBe(newContributor.Username);
            expectedContributor.Email.ShouldBe(newContributor.Email);
            expectedContributor.Group.ShouldBe(newContributor.Group);
        }
        
    }
}