using System;
using Microsoft.AspNetCore.Http;
using Moq;
using OWArcadeBackend.Validators.Contributor;
using Xunit;

namespace OWArcadeBackend.Tests.Validators.Contributor
{
    public class ContributorAvatarValidatorTest
    {
        [Fact]
        public void TestAvatarValidator_Success()
        {
            // Arrange
            var avatar = new Mock<IFormFile>();
            long avatarSize = Convert.ToInt64("720000");
            avatar.SetupGet(x => x.ContentType).Returns("image/jpeg");
            avatar.SetupGet(x => x.Length).Returns(avatarSize);
            
            // Act
            var result = new ContributorAvatarValidator().Validate(avatar.Object);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void TestAvatarValidator_Invalid_FormatAndSize()
        {
            // Arrange
            var avatar = new Mock<IFormFile>();
            long avatarSize = Convert.ToInt64("1000000");
            avatar.SetupGet(x => x.ContentType).Returns("image/webp");
            avatar.SetupGet(x => x.Length).Returns(avatarSize);
            
            // Act
            var result = new ContributorAvatarValidator().Validate(avatar.Object);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count);
            Assert.Equal("File size exceeds the 750kb limit", result.Errors[0].ErrorMessage);
            Assert.Equal("File type is not allowed. Must be JPG/PNG", result.Errors[1].ErrorMessage);
        }
    }
}