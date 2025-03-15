using AI_Recruitment_Assistant.Application.Abstractions.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace AI_Recruitment_Assistant.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendScheduleEmailAsync(string email, DateTime interviewTime, string meetLink)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(
            _config["EmailConfig:DisplayName"],
            _config["EmailConfig:From"]
        ));
        emailMessage.To.Add(MailboxAddress.Parse(email));
        emailMessage.Subject = "Your Interview Schedule";

        var bodyBuilder = new BodyBuilder
        {
            TextBody = $"Your interview is scheduled for {interviewTime:yyyy-MM-dd HH:mm}\nMeeting Link: {meetLink}",
            HtmlBody = $@"
                <h1>Interview Scheduled</h1>
                <p>Your interview is scheduled for <strong>{interviewTime:yyyy-MM-dd HH:mm}</strong></p>
                <p>Join using this link: <a href='{meetLink}'>{meetLink}</a></p>
            "
        };

        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _config["EmailConfig:Host"],
            _config.GetValue<int>("EmailConfig:Port"),
            SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _config["EmailConfig:Username"],
            _config["EmailConfig:Password"]
        );

        await smtp.SendAsync(emailMessage);
        await smtp.DisconnectAsync(true);
    }
}