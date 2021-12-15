using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShadowTracker.Models;

namespace ShadowTracker.Services.Interfaces
{
    public interface IBTNotificationService
    {
        public Task AddNotificationAsync(Notification notification);

        public Task<List<Notification>> GetReceivedNotificationsAsync(string userId);

        public Task<List<Notification>> GetSentNotificationsAsync(string userId);

        public Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role);

        public Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members);

        public Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject);

        public Task<List<Notification>> GetChatAlertsAsync(string userId);

        public Task<List<Notification>> GetNotificationAlertsAsync(string userId);
    }
}