using System.Collections.Concurrent;
using Contracts.NotificationService;

namespace NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ConcurrentDictionary<int, Stack<Notification>> _notifications = new();

        public Notification? GetLastByAccountId(int accountId)
        {
            if (!_notifications.TryGetValue(accountId, out Stack<Notification>? notifications))
            {
                return null;
            }

            return notifications.Peek();
        }

        public IList<Notification> GetAllByAccountId(int accountId)
        {
            if (_notifications.TryGetValue(accountId, out Stack<Notification>? notifications))
            { 
                return [.. notifications]; 
            }

            return [];
        }

        public void Save(int accountId, Notification notification)
        {
            if (!_notifications.TryGetValue(accountId, out Stack<Notification>? notifications))
            {
                notifications = new Stack<Notification>();
            }

            notifications.Push(notification);
            _notifications.TryAdd(accountId, notifications);
        }
    }
}
