using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Factories.Interfaces;
using OverwatchArcade.API.Services.ContributorService;
using OverwatchArcade.API.Utility;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Persistence;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Services
{
    public class ContributorServiceTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IFileProvider> _fileProviderMock;
        private readonly Mock<ILogger<ContributorService>> _loggerMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private readonly Mock<IValidator<ContributorAvatarDto>> _contributorAvatarValidatorMock;
        private readonly Mock<IValidator<ContributorProfileDto>> _contributorProfileValidatorMock;
        private readonly Mock<IServiceResponseFactory<ContributorDto>> _serviceResponseFactoryMock;

        private readonly ContributorService _contributorService;
        private Contributor _contributor;
        private ContributorDto _contributorDto;

        public ContributorServiceTest()
        {
            _loggerMock = new Mock<ILogger<ContributorService>>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _fileProviderMock = new Mock<IFileProvider>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _contributorAvatarValidatorMock = new Mock<IValidator<ContributorAvatarDto>>();
            _contributorProfileValidatorMock = new Mock<IValidator<ContributorProfileDto>>();
            _serviceResponseFactoryMock = new Mock<IServiceResponseFactory<ContributorDto>>();
            ConfigureTestData();

            _contributorService = new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _fileProviderMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object,
                _serviceResponseFactoryMock.Object);
        }

        private void ConfigureTestData()
        {
            _contributor = new Contributor
            {
                Id = new Guid("62A1EA25-BD54-47F9-BACE-B03A1B8AEC95"),
                Username = "System",
                Avatar = ImageConstants.DefaultAvatar
            };

            _contributorDto = new ContributorDto
            {
                Username = "System"
            };
        }

        [Fact]
        public void Constructor()
        {
            var constructor = new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _fileProviderMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object,
                _serviceResponseFactoryMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() =>
                new ContributorService(null, _unitOfWorkMock.Object, _fileProviderMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object));
            
            Should.Throw<ArgumentNullException>(() =>
                new ContributorService(_mapperMock.Object, null, _fileProviderMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object));
            
            Should.Throw<ArgumentNullException>(() =>
                new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, null, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object));
            
            Should.Throw<ArgumentNullException>(() =>
                new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _fileProviderMock.Object, null, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object));
            
            Should.Throw<ArgumentNullException>(() =>
                new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _fileProviderMock.Object, _loggerMock.Object, null, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object));
            
            Should.Throw<ArgumentNullException>(() =>
                new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _fileProviderMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, null, _contributorProfileValidatorMock.Object, _serviceResponseFactoryMock.Object));
            
            Should.Throw<ArgumentNullException>(() =>
                new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _fileProviderMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, null, _serviceResponseFactoryMock.Object));
            
            Should.Throw<ArgumentNullException>(() =>
                new ContributorService(_mapperMock.Object, _unitOfWorkMock.Object, _fileProviderMock.Object, _loggerMock.Object, _webHostEnvironmentMock.Object, _contributorAvatarValidatorMock.Object, _contributorProfileValidatorMock.Object, null));
        }

        [Fact]
        public async Task GetAllContributors_Returns_ListOfContributors()
        {
            // Arrange
            var otherContributor = new Contributor()
            {
                Username = "Tester",
                Stats = new ContributorStats()
                {
                    ContributionDays = new List<DateTime>()
                    {
                        DateTime.Now
                    }
                }
            };
            var otherContributorDto = new ContributorDto()
            {
                Username = "Tester"
            };
            var contributors = new List<Contributor> { _contributor, otherContributor };
            var contributorDtos = new List<ContributorDto> { _contributorDto, otherContributorDto };

            _unitOfWorkMock.Setup(x => x.ContributorRepository.GetAll()).ReturnsAsync(contributors);
            _mapperMock.Setup(x => x.Map<List<ContributorDto>>(contributors)).Returns(contributorDtos);

            // Act
            var result = await _contributorService.GetAllContributors();

            // Assert
            result.Data.ShouldBeOfType<List<ContributorDto>>();
            Assert.Equal(2, result.Data.Count);
            Assert.Null(otherContributor.Profile);
        }

        [Fact]
        public void GetContributorByUsername_Returns_ContributorDto()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.ContributorRepository.FirstOrDefault(It.IsAny<Expression<Func<Contributor,bool>>>())).Returns(_contributor);
            _mapperMock.Setup(x => x.Map<ContributorDto>(_contributor)).Returns(_contributorDto);

            // Act
            _contributorService.GetContributorByUsername(_contributor.Username);

            // Assert
            _serviceResponseFactoryMock.Verify(x => x.Create(_contributorDto), Times.Once);
        }
        
        [Fact]
        public void GetContributorByUsername_UnknownContributor_NotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.ContributorRepository.FirstOrDefault(It.IsAny<Expression<Func<Contributor,bool>>>())).Returns((Contributor) null);

            // Act
            _contributorService.GetContributorByUsername(_contributor.Username);

            // Assert
            _serviceResponseFactoryMock.Verify(x => x.Error(404, $"Contributor {_contributor.Username} not found"), Times.Once);
        }

        [Fact]
        public async Task SaveProfile_Fails_Validation()
        {
            // Arrange
            var contributorProfileDto = new ContributorProfileDto()
            {
                Personal = new About()
                {
                    Text = "I love unit tests!"
                }
            };
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(false);
            _contributorProfileValidatorMock.Setup(x => x.ValidateAsync(contributorProfileDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);
            
            // Act
            await _contributorService.SaveProfile(contributorProfileDto, _contributor.Id);

            // Assert
            _serviceResponseFactoryMock.Verify(x => x.Error(403, It.IsAny<string[]>()));
        }
        
        [Fact]
        public async Task SaveProfile_Error_CouldntSave()
        {
            // Arrange
            var contributorProfileDto = new ContributorProfileDto()
            {
                Personal = new About()
                {
                    Text = "I love unit tests!"
                }
            };
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            _contributorProfileValidatorMock.Setup(x => x.ValidateAsync(contributorProfileDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.FirstOrDefaultASync(It.IsAny<Expression<Func<Contributor, bool>>>())).ReturnsAsync((Contributor) null);
            
            // Act
            await _contributorService.SaveProfile(contributorProfileDto, _contributor.Id);

            // Assert
            _serviceResponseFactoryMock.Verify(x => x.Error(500, $"Contributor with userid {_contributor.Id} not found"));
        }
        
        [Fact]
        public async Task SaveProfile_Saves_Profile()
        {
            // Arrange
            var contributorProfileDto = new ContributorProfileDto()
            {
                Personal = new About()
                {
                    Text = "I love unit tests!"
                }
            };
            var contributorProfile = new ContributorProfile()
            {
                Personal = new About()
                {
                    Text = "I love unit tests!"
                }
            };
            
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            _contributorProfileValidatorMock.Setup(x => x.ValidateAsync(contributorProfileDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.FirstOrDefaultASync(It.IsAny<Expression<Func<Contributor, bool>>>())).ReturnsAsync(_contributor);
            _mapperMock.Setup(x => x.Map<ContributorProfile>(contributorProfileDto)).Returns(contributorProfile);
            _mapperMock.Setup(x => x.Map<ContributorDto>(_contributor)).Returns(_contributorDto);
            
            // Act
            await _contributorService.SaveProfile(contributorProfileDto, _contributor.Id);

            // Assert
            _serviceResponseFactoryMock.Verify(x => x.Create(_contributorDto));
            _unitOfWorkMock.Verify(x => x.Save(), Times.Once);
        }

        [Fact]
        public async Task SaveAvatar_Fails_Validation()
        {
            // Arrange
            var avatarMock = new Mock<IFormFile>();
            var contributorAvatarDto = new ContributorAvatarDto
            {
                Avatar = avatarMock.Object
            };
            
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(false);
            _contributorAvatarValidatorMock.Setup(x => x.ValidateAsync(contributorAvatarDto, It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);
            
            // Act
            await _contributorService.SaveAvatar(contributorAvatarDto, _contributor.Id);

            // Assert
            _serviceResponseFactoryMock.Verify(x => x.Error(It.IsAny<int>(), It.IsAny<string[]>()));
        }

        [Fact]
        public async Task SaveAvatar_UnknownContributor_NotFound()
        {
            // Arrange
            var avatarMock = new Mock<IFormFile>();
            var contributorAvatarDto = new ContributorAvatarDto
            {
                Avatar = avatarMock.Object
            };
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            _contributorAvatarValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<ContributorAvatarDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.FirstOrDefaultASync(It.IsAny<Expression<Func<Contributor, bool>>>())).ReturnsAsync((Contributor)null);

            // Act
            await _contributorService.SaveAvatar(contributorAvatarDto, _contributor.Id);
            
            // Assert
            _serviceResponseFactoryMock.Verify(x => x.Error(404, $"Contributor with userid {_contributor.Id} not found"));
        }

        [Fact]
        public async Task SaveAvatar()
        {
            // Arrange
            _contributor.Avatar = "avatar.jpg";
            var avatarMock = new Mock<IFormFile>();
            var contributorAvatarDto = new ContributorAvatarDto
            {
                Avatar = avatarMock.Object
            };
            var validateResultMock = new Mock<ValidationResult>();
            validateResultMock.Setup(x => x.IsValid).Returns(true);
            _contributorAvatarValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<ContributorAvatarDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(validateResultMock.Object);
            _unitOfWorkMock.Setup(x => x.ContributorRepository.FirstOrDefaultASync(It.IsAny<Expression<Func<Contributor, bool>>>())).ReturnsAsync(_contributor);
            _mapperMock.Setup(x => x.Map<ContributorDto>(_contributor)).Returns(_contributorDto);

            // Act
            await _contributorService.SaveAvatar(contributorAvatarDto, _contributor.Id);
            
            // Assert
            _serviceResponseFactoryMock.Verify(x => x.Create(_contributorDto));
        }
    }
}