namespace OrderManagementSystem.APIs.EmailService
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string from, string recipients, string subject, string body);
    }
}
