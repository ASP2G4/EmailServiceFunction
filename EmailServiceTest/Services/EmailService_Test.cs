using EmailServiceFunction.Dtos;
using EmailServiceFunction.Models;
using EmailServiceFunction.Services;
using Moq;

namespace EmailService.Tests.Services
{
    public class EmailService_Test
    {
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly IEmailService _emailService;

        public EmailService_Test()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _emailService = _emailServiceMock.Object;
        }

        [Fact]
        public async Task SendEmail_ShouldReturnSuccess()
        {
            // arrange
            var emailMessage = new EmailMessageModel
            {
                Recipients = new List<string> { "test@test.com" },
                Subject = "Test",
                PlainText = "Test",
                HTML = "<p> Test </p>"
            };

            var result = new EmailServiceResult
            {
                Succeeded = true,
                Error = null
            };

            _emailServiceMock
                .Setup(s => s.SendEmailAsync(It.IsAny<EmailMessageModel>()))
                .ReturnsAsync(result);

            // act
            var response = await _emailService.SendEmailAsync(emailMessage);

            // assert
            Assert.True(response.Succeeded);
            Assert.Null(response.Error);
        }
    }
}

