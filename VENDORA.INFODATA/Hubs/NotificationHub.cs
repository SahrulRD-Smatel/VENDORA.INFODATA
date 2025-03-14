using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace VENDORA.INFODATA.Hubs
{
    public class NotificationHub : Hub
    {
        private static ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            string userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            string userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.TryRemove(userId, out _);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(int userId, string message)
        {
            if (_userConnections.TryGetValue(userId.ToString(), out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
        }
    }
}
