using System.Threading.Tasks;

namespace BirthList.Shared.Services
{
    public interface IMailService
    {
        Task<bool> ExecuteSendEmail(PurchasedPresent presentInfo);
        Task<bool> SendEmail(PurchasedPresent presentInfo);
    }
}