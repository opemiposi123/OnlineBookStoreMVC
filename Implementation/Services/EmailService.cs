using Microsoft.Extensions.Options;
using MimeKit;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Services
{
    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _hostenv;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly string _apiKey;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IWebHostEnvironment hostenv, IOptions<EmailConfiguration> emailConfiguration, IConfiguration configuration, ILogger<EmailService> logger)
        {
            _hostenv = hostenv;
            _emailConfiguration = emailConfiguration.Value;
            _apiKey = configuration.GetValue<string>("MailConfig:mailApikey");
            _logger = logger;
        }

        public async Task<BaseResponse<MailRecieverDto>> SendMessageToUserAsync(UserRequestModel user)
        {
            _logger.LogInformation("SendMessageToUserAsync called for user: {UserName}", user.FullName);

            var mailReceiverRequest = new MailRecieverDto
            {
                Email = user.Email,
                Name = user.FullName,
            };

            string emailBody = GenerateWelcomeEmailBody(user.FullName);

            var mailRequest = new MailRequests
            {
                Body = emailBody,
                Title = "WELCOME TO ABDULMUHEEZ ONLINE BOOKSTORE",
                HtmlContent = emailBody,
                ToEmail = user.Email
            };

            try
            {
                await SendEmailAsync(mailReceiverRequest, mailRequest);
                _logger.LogInformation("Email sent successfully to: {Email}", mailReceiverRequest.Email);
                return new BaseResponse<MailRecieverDto>
                {
                    Message = "Email sent successfully",
                    Success = true,
                    Data = mailReceiverRequest
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to: {Email}", mailReceiverRequest.Email);
                return new BaseResponse<MailRecieverDto>
                {
                    Message = $"Failed to send notification: {ex.Message}",
                    Success = false,
                };
            }
        }

        private string GenerateWelcomeEmailBody(string userName)
        {
            return $"<p>Welcome to AbdulMuheez Online Bookstore!</p>\r\n" +
                     $"<p>Dear {userName},</p>\r\n" +
                     $"<p>Thank you for registering with us. We are thrilled to have you as a part of our community.</p>\r\n" +
                     $"<p>At AbdulMuheez, we are committed to providing you with a wide range of books and exceptional service.</p>\r\n" +
                     $"<p>Here’s what you can do next:</p>\r\n" +
                     $"<ul>" +
                     $"<li><strong>Explore Our Collection:</strong> Browse our extensive catalog of books across various genres.</li>" +
                     $"<li><strong>Update Your Profile:</strong> Ensure your account information is accurate and up-to-date.</li>" +
                     $"<li><strong>Subscribe to Our Newsletter:</strong> Stay informed about the latest releases, promotions, and exclusive offers.</li>" +
                     $"</ul>" +
                     $"<p>We look forward to providing you with an enjoyable and fulfilling reading experience.</p>\r\n" +
                     $"<p>Happy reading!</p>\r\n" +
                     $"<p>Best regards,</p>\r\n" +
                     $"<p>abdulmuheezonlinebookstore.com</p>\r\n" +
                     $"<p>abdulmuheezalabi@gmail.com</p>\r\n" +
                     $"<p><strong>The AbdulMuheez Online Bookstore Team</strong></p>";
        }

        public async Task SendEmailClient(string msg, string title, string email)
        {
            _logger.LogInformation("SendEmailClient called with email: {Email}, subject: {Title}", email, title);

            if (string.IsNullOrEmpty(msg))
            {
                _logger.LogError("Email message content cannot be null or empty");
                throw new ArgumentNullException(nameof(msg), "Email message content cannot be null or empty");
            }

            var message = new MimeMessage();
            message.To.Add(MailboxAddress.Parse(email));
            message.From.Add(new MailboxAddress(_emailConfiguration.EmailSenderName, _emailConfiguration.EmailSenderAddress));
            message.Subject = title;

            message.Body = new TextPart("html")
            {
                Text = msg
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    _logger.LogInformation("Connecting to SMTP server at {SMTPServerAddress}", _emailConfiguration.SMTPServerAddress);
                    await client.ConnectAsync(_emailConfiguration.SMTPServerAddress, _emailConfiguration.SMTPServerPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_emailConfiguration.EmailSenderAddress, _emailConfiguration.EmailSenderPassword);
                    await client.SendAsync(message);
                    _logger.LogInformation("Email sent successfully to: {Email}", email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while sending email to: {Email}", email);
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        public async Task<bool> SendEmailAsync(MailRecieverDto model, MailRequests request)
        {
            _logger.LogInformation("SendEmailAsync called for email: {Email}", model.Email);

            try
            {
                if (string.IsNullOrWhiteSpace(request.HtmlContent))
                {
                    _logger.LogError("Email content cannot be null or empty");
                    throw new ArgumentNullException(nameof(request.HtmlContent), "Email content cannot be null or empty");
                }

                await SendEmailClient(request.HtmlContent, request.Title, model.Email);
                _logger.LogInformation("Email content sent successfully to: {Email}", model.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending email to: {Email}", model.Email);
                throw new Exception("There was an error while sending email", ex);
            }
        }

        public async Task NotifyAdminAsync(string message)
        {
            var adminEmail = _emailConfiguration.AdminEmail;

            string emailBody = GenerateAdminNotificationEmailBody();

            var mailRequest = new MailRequests
            {
                Body = emailBody,
                Title = "Alert: Email Sending To register Failure",
                HtmlContent = emailBody,
                ToEmail = adminEmail
            };

            try
            {
                await SendEmailClient(mailRequest.HtmlContent, mailRequest.Title, adminEmail);
                _logger.LogInformation("Admin notified successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to notify admin.");
            }
        }

        private string GenerateAdminNotificationEmailBody()
        {
            return $"Dear Admin,</p>\r\n" +
                     $"<p>I hope this message finds you well.</p>\r\n" +
                     $"<p>I am reaching out to report an issue regarding user registrations.</p>\r\n" +
                     $"<p>It has come to my attention that users who are registering are not receiving their confirmation emails.</p>\r\n" +
                     $"<p>This is preventing them from completing their registration process and accessing their accounts.</p>\r\n" +
                     $"<p>Could you please investigate this issue as soon as possible?</p>\r\n" +
                     $"<p>Ensuring that confirmation emails are sent and received is crucial for the smooth operation of our registration system</p>\r\n" +
                     $"<p>If you need any additional details or if there are specific steps I should follow, please let me know.</p>\r\n" +
                     $"<p>Your prompt assistance with this matter would be greatly appreciated.</p>\r\n" +
                     $"<p>Thank you for your attention and support.</p>\r\n" +
                     $"<p>Best regards,</p>\r\n" +
                     $"<p><strong>The Online BookStore Server</strong></p>";
        }

    }
}
