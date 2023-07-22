using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Sockets.GrainInterfaces;

namespace Sockets.Hubs
{
	public interface IHub
	{
	}

	// A SignalR Hub. https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-7.0
	public class MyHub : Hub, IHub
	{
	    private readonly IClusterClient _clusterClient;
	    private readonly ILogger<MyHub> _logger;

	    public MyHub(IClusterClient clusterClient, ILogger<MyHub> logger)
	    {
			_clusterClient = clusterClient ?? throw new ArgumentNullException(nameof(clusterClient));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	    }

		public async void JoinRoom(string name, string room) {
			var userGrain = _clusterClient.GetGrain<IUser>(Context.ConnectionId);
			await userGrain.SetName(name);
			await userGrain.EnterChatRoom(room);
		}

		public async void LeaveRoom() {
			var userGrain = _clusterClient.GetGrain<IUser>(Context.ConnectionId);
			await userGrain.LeaveChatRoom();
		}

	    public async void SendMessage(string message) {
			var time = DateTimeOffset.Now;
			var userGrain = _clusterClient.GetGrain<IUser>(Context.ConnectionId);
			await userGrain.SendMessage(message, time);
	    }

		public async void Typing() {
			var userGrain = _clusterClient.GetGrain<IUser>(Context.ConnectionId);
			await userGrain.StartTyping();
		}

		public async void StoppedTyping() {
			var userGrain = _clusterClient.GetGrain<IUser>(Context.ConnectionId);
			await userGrain.StoppedTyping();
		}

	    public override async Task OnConnectedAsync()
	    {
			_logger.LogInformation($"{Context.ConnectionId} connected.");
			Console.WriteLine($"{Context.ConnectionId} connected.");
			await base.OnConnectedAsync();
	    }

	    public override async Task OnDisconnectedAsync(Exception? exception)
	    {
			_logger.LogInformation(exception, $"{nameof(OnDisconnectedAsync)} disconnected.");
			var userGrain = _clusterClient.GetGrain<IUser>(Context.ConnectionId);
			await userGrain.LeaveChatRoom();
			await base.OnDisconnectedAsync(exception);
	    }
	}
}

