using Contracts.NotificationService;

namespace NotificationService.Services
{
    public interface INotificationService
    {
        Notification? GetLastByAccountId(int accountId);

        IList<Notification> GetAllByAccountId(int accountId);

        void Save(int accountId, Notification notification);
    }
}
