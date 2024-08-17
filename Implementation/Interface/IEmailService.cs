using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Models.RequestModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IEmailService
    {
        Task SendEmailClient(string msg, string title, string email);
        Task<BaseResponse<MailRecieverDto>> SendMessageToUserAsync(UserRequestModel user);
        Task<bool> SendEmailAsync(MailRecieverDto model, MailRequests request);
        Task NotifyAdminAsync(string message);
        //Task SendPasswordResetCodeAsync(string email, string code);

    }
}
