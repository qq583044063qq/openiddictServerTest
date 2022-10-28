using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TestApi1.SignalRHubs
{
    [Authorize]
    [Authorize(Policy = UserPolicyName.User)]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }
        public async Task SendMessage(string user, string message)
            => await Clients.All.SendAsync("ReceiveMessage", user, message);
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation(message: "客户端连接...当前客户端id:{clientId}", Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation(message: "客户端断开连接...当前客户端id:{clientId}", Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
