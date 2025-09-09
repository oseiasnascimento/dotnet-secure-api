using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Guardian.Application.Shared.Contracts;
using Guardian.Application.Shared.Dtos;

namespace Guardian.Infrastructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(SendMailRequest request)
        {
            string host = _configuration["EmailSettings:Host"];
            _ = int.TryParse(_configuration["EmailSettings:Port"], out int port);
            string email = _configuration["EmailSettings:Email"];
            string password = Environment.GetEnvironmentVariable("EMAIL_PASS");

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Guardian", email));
            emailMessage.To.Add(new MailboxAddress("", request.To));
            emailMessage.Subject = request.Subject;

            string emailBody = request.Body;

            // Se TemplatePath não for nulo, significa que o e-mail possui um template HTML.
            if (!string.IsNullOrEmpty(request.TemplatePath))
            {
                string pathToHtmlFile = Path.Combine("wwwroot", "Templates", "Account", request.TemplatePath);

                if (File.Exists(pathToHtmlFile))
                {
                    using StreamReader reader = new(pathToHtmlFile);
                    emailBody = await reader.ReadToEndAsync();

                    foreach (KeyValuePair<string, string> keyValuePair in request.Parameters)
                    {
                        emailBody = emailBody.Replace(keyValuePair.Key, keyValuePair.Value);
                    }
                }
                else
                {
                    throw new FileNotFoundException($"O template {request.TemplatePath} não foi encontrado.");
                }
            }

            BodyBuilder bodyBuilder = new()
            {
                HtmlBody = emailBody
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(email, password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}