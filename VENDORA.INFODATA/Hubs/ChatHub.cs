using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace VENDORA.INFODATA.Hubs
{
    public class ChatHub : Hub
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

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.TryRemove(userId, out _);
            }
            return base.OnDisconnectedAsync(exception);
        }

        // 🔹 Gabungan: Chat Pribadi (Direct) & Grup (Thread)
        public async Task SendMessage(int threadId, int senderId, int receiverId, string message)
        {
            if (_userConnections.TryGetValue(receiverId.ToString(), out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", new { threadId, senderId, message });
            }
            else
            {
                // Kirim ke semua yang tergabung dalam thread
                await Clients.Group($"thread-{threadId}").SendAsync("ReceiveMessage", new { threadId, senderId, message });
            }
        }

        public async Task JoinThread(int threadId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"thread-{threadId}");
        }

        public async Task LeaveThread(int threadId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"thread-{threadId}");
        }
    }
}
