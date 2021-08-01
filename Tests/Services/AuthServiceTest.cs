using System;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
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

        public AuthServiceTest()
        {
            _configurationMock = new Mock<IConfiguration>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _authRepositoryMock = new Mock<IAuthRepository>();
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new AuthService(_configurationMock.Object, _mapperMock.Object, _unitOfWorkMock.Object, _loggerMock.Object, _authRepositoryMock.Object, _webHostEnvironmentMock.Object);
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
                _webHostEnvironmentMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                null,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _webHostEnvironmentMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _mapperMock.Object,
                null,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                _webHostEnvironmentMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                null,
                _authRepositoryMock.Object,
                _webHostEnvironmentMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                null,
                _webHostEnvironmentMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new AuthService(
                _configurationMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _authRepositoryMock.Object,
                null
            ));
        }
        
        
    }
}