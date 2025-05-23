﻿using Azure;
using Azure.Communication.Email;
using EmailServiceFunction.Dtos;
using EmailServiceFunction.Models;
using Microsoft.Extensions.Options;

namespace EmailServiceFunction.Services
{
    public interface IEmailService
    {
        Task<EmailServiceResult> SendEmailAsync(EmailMessageModel message);
    }
    public class EmailService(EmailClient client, IOptions<AzureCommunicationsSettings> options) : IEmailService
    {
        private readonly EmailClient _client = client;
        private readonly AzureCommunicationsSettings _settings = options.Value;

        public async Task<EmailServiceResult> SendEmailAsync(EmailMessageModel message)
        {
            try
            {
                var recipients = message.Recipients.Select(email => new EmailAddress(email)).ToList();

                var emailMessage = new EmailMessage(
                    senderAddress: _settings.SenderAddress,
                    content: new EmailContent(message.Subject)
                    {
                        PlainText = message.PlainText,
                        Html = message.HTML
                    },
                    recipients: new EmailRecipients(recipients));

                var response = await _client.SendAsync(WaitUntil.Completed, emailMessage);

                return response.HasCompleted
                    ? new EmailServiceResult { Succeeded = true }
                    : new EmailServiceResult { Succeeded = false, Error = "The email send-confirmation couldn't be confirmed." };
            }
            catch (RequestFailedException ex)
            {
                return new EmailServiceResult
                {
                    Succeeded = false,
                    Error = $"An error occurred ({ex.Status}: {ex.Message}"
                };
            }

            catch (Exception ex)
            {
                return new EmailServiceResult
                {
                    Succeeded = false,
                    Error = $"An unexpected error occured: {ex.Message}"
                };
            }
        }
    }
}
