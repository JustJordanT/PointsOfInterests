using CityInfo.API.Services.Interfaces;

namespace CityInfo.API.Services;

public class CloudMailService : IMailService
{
    public readonly string _mailTo = string.Empty;
    public readonly string _mailFrom = string.Empty;


    public CloudMailService(IConfiguration configuration)
    {
        // TODO This is not working.
        _mailTo = configuration["mailSettings:mailToAddress"];
        _mailFrom = configuration["mailSettings:mailFromAddress"];
    }
    
    public void Send(string subject, string message)
    {
        // Sending mail - output to console.
        Console.WriteLine($"Sending mail to: {_mailTo}; from: {_mailFrom} with {nameof(CloudMailService)}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}