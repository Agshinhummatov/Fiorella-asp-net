namespace EntityFramework_Slider.Services.Interfaces
{
    public interface IEmailService
    {
       void Send(string to, string subject, string html, string from = null);
    }
}
