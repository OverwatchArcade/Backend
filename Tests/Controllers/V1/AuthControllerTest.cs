using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OWArcadeBackend.Controllers.V1.Contributor;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models;
using OWArcadeBackend.Services.AuthService;
using OWArcadeBackend.Services.ContributorService;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Controllers.V1
{
    public class AuthControllerTest
    {
        private Mock<IAuthService> _authServiceMock;
        private Mock<IContributorService> _contributorServiceMock;
        
        private Guid _userId;
        private ClaimsPrincipal _claimsPrincipalUser;
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
        }
        
        [Fact]
        public void TestConstructor()
        {
            var constructor = new AuthController(_authServiceMock.Object, _contributorServiceMock.Object);
            Assert.NotNull(constructor);
        }
        
        [Fact]
        public void TestConstructorFunction_throws_Exception()
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
        public async Task TestLogin_Succesfull()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2000");
            const string code = "12345";
            var serviceResponse = new ServiceResponse<string>()
            {
                Data = "12345",
                Time = date
            };
            var expectedResponse = new ServiceResponse<string>()
            {
                Data = "12345",
                Time = date
            };

            _authServiceMock.Setup(x => x.RegisterAndLogin(code)).ReturnsAsync(serviceResponse);

            // Act
            var result = await new AuthController(_authServiceMock.Object, _contributorServiceMock.Object).Login(code);
            
            // Assert
            result.ShouldBeOfType<ObjectResult>();
            var returnedValue = result as ObjectResult;
            AssertActionResult(returnedValue, expectedResponse);
        }

        [Fact]
        public async Task TestLogin_EmptyCode_ThrowsBadRequest()
        {
            // Arrange
            const string code = "";
            
            // Act
            var result = await new AuthController(_authServiceMock.Object, _contributorServiceMock.Object).Login(code);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void TestLogout()
        {
            // Arrange
            // Act
            var result = new AuthController(_authServiceMock.Object, _contributorServiceMock.Object).Logout();
            
            // Assert
            result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task TestInfo()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2000");
            var serviceResponse = new ServiceResponse<ContributorDto>()
            {
                Data = new ContributorDto()
                {
                    Username = _username
                },
                Time = date
            };
            var expectedResponse = new ServiceResponse<ContributorDto>()
            {
                Data = new ContributorDto()
                {
                    Username = _username
                },
                Time = date
            };
            _contributorServiceMock.Setup(x => x.GetContributorByUsername(_username, false)).ReturnsAsync(serviceResponse);
            
            // Act
            var controller = new AuthController(_authServiceMock.Object, _contributorServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = _claimsPrincipalUser}
                }
            };
            var actionResult = await controller.Info();
            
            // Assert
            _contributorServiceMock.Verify(x => x.GetContributorByUsername(_username, false));
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public async Task TestSaveProfile()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2000");
            var contributorProfile = new ContributorProfile();
            var serviceResponse = new ServiceResponse<ContributorDto>()
            {
                Data = new ContributorDto()
                {
                    Username = _username
                },
                Time = date
            };
            var expectedResponse = new ServiceResponse<ContributorDto>()
            {
                Data = new ContributorDto()
                {
                    Username = _username
                },
                Time = date
            };
            _authServiceMock.Setup(x => x.SaveProfile(contributorProfile, _userId)).ReturnsAsync(serviceResponse);
            
            // Act
            var controller = new AuthController(_authServiceMock.Object, _contributorServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = _claimsPrincipalUser}
                }
            };
            var actionResult = await controller.SaveProfile(contributorProfile);
            
            // Assert
            _authServiceMock.Verify(x => x.SaveProfile(contributorProfile, _userId));
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
        
        [Fact]
        public async Task TestUploadAvatar()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2000");
            var contributorAvatar = new ContributorAvatarDto()
            {
                Avatar = new Mock<IFormFile>().Object
            };
            var serviceResponse = new ServiceResponse<ContributorDto>()
            {
                Data = new ContributorDto()
                {
                    Username = _username
                },
                Time = date
            };
            var expectedResponse = new ServiceResponse<ContributorDto>()
            {
                Data = new ContributorDto()
                {
                    Username = _username
                },
                Time = date
            };
            _authServiceMock.Setup(x => x.UploadAvatar(contributorAvatar, _userId)).ReturnsAsync(serviceResponse);
            
            // Act
            var controller = new AuthController(_authServiceMock.Object, _contributorServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = _claimsPrincipalUser}
                }
            };
            var actionResult = await controller.UploadAvatar(contributorAvatar);
            
            // Assert
            _authServiceMock.Verify(x => x.UploadAvatar(contributorAvatar, _userId));
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
    }
}