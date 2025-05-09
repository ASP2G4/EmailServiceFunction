using Azure.Messaging.ServiceBus;
using EmailServiceFunction.Models;
using EmailServiceFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EmailServiceFunction.Functions;

public class EmailFunction
{
    private readonly ILogger<EmailFunction> _logger;
    private readonly IEmailService _emailService;

    public EmailFunction(ILogger<EmailFunction> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    [Function(nameof(EmailFunction))]
    public async Task Run(
        [ServiceBusTrigger("email-queue", Connection = "AzureServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        try
        {
            var emailMessage = message.Body.ToObjectFromJson<EmailMessageModel>();

            _logger.LogInformation("Processing email request for: {email}", emailMessage.Recipients[0]);

            var result = await _emailService.SendEmailAsync(emailMessage);

            if (result.Succeeded)
            {
                await messageActions.CompleteMessageAsync(message);
            }
            else
            {
                _logger.LogError("Failed to send email: {error}", result.Error);
                await messageActions.DeadLetterMessageAsync(message, null, "EmailSendError", result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error processing message: {error}", ex.Message);
            await messageActions.DeadLetterMessageAsync(message, null, "ProcessingError", ex.Message);
        }
    }
}