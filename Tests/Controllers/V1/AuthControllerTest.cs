using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OverwatchArcade.API.Controllers.V1.Contributor;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Services.AuthService;
using OverwatchArcade.API.Services.ContributorService;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Controllers.V1
{
    public class AuthControllerTest
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IContributorService> _contributorServiceMock;
        
        private Guid _userId;
        private ClaimsPrincipal _claimsPrincipalUser;
        private AuthController _authController;
        private string _username;


        public AuthControllerTest()
        {
            _authServiceMock = new Mock<IAuthService>();
            _contributorServiceMock = new Mock<IContributorService>();
            
            PrepareMock();
        }

        private void PrepareMock()
        {
            _userId = new Guid("01F07C7B-F24D-4C7A-A080-F970C75F6691");
            _username = "system";
            _claimsPrincipalUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString()),
                new Claim(ClaimTypes.Name, _username)
            }));
            _authController = new AuthController(_authServiceMock.Object, _contributorServiceMock.Object);
        }
        
        [Fact]
        public void Constructor()
        {
            var constructor = new AuthController(_authServiceMock.Object, _contributorServiceMock.Object);
            Assert.NotNull(constructor);
        }
        
        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new AuthController(
                null,
                _contributorServiceMock.Object
            ));
        
            Should.Throw<ArgumentNullException>(() => new AuthController(
                _authServiceMock.Object,
                null
            ));
        }
        
        private static void AssertActionResult<T>(ObjectResult result, ServiceResponse<T> expectedResponse)
        {
            result.ShouldNotBeNull();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<ServiceResponse<T>>();
            result.Value.ShouldBeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task Login_Successful()
        {
            // Arrange
            const string code = "12345";
            const string discordRedirectUri = "https://site/auth/callback";
            var serviceResponse = new ServiceResponse<string>()
            {
                Data = "12345"
            };
            var expectedResponse = new ServiceResponse<string>()
            {
                Data = "12345"
            };

            _authServiceMock.Setup(x => x.RegisterAndLogin(code,discordRedirectUri)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _authController.Login(code, discordRedirectUri);
            
            // Assert
            result.ShouldBeOfType<ObjectResult>();
            var returnedValue = result as ObjectResult;
            AssertActionResult(returnedValue, expectedResponse);
        }

        [Fact]
        public async Task Login_EmptyCode_ThrowsBadRequest()
        {
            // Arrange
            const string code = "";
            const string discordRedirectUri = "https://site/auth/callback";

            // Act
            var result = await _authController.Login(code, discordRedirectUri);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void Logout()
        {
            // Arrange & Act
            var result = new AuthController(_authServiceMock.Object, _contributorServiceMock.Object).Logout();
            
            // Assert
            result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public void GetInfo()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<ContributorDto>()
            {
                Data = new ContributorDto()
                {
                    Username = _username
                }
            };
            var expectedResponse = new ServiceResponse<ContributorDto>()
            {
                Data = new ContributorDto()
                {
                    Username = _username
                }
            };
            _contributorServiceMock.Setup(x => x.GetContributorByUsername(_username)).Returns(serviceResponse);
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _claimsPrincipalUser }
            };
            
            // Act
            var actionResult = _authController.Info();
            
            // Assert
            _contributorServiceMock.Verify(x => x.GetContributorByUsername(_username));
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
    }
}