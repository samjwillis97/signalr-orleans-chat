using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Sockets.GrainInterfaces;
using Sockets.Hubs;

namespace Sockets.Grains
{
	public class UserGrain : Grain, IUser
	{
        private readonly IClusterClient _client;
        private readonly IHubContext<MyHub> _hubContext;
        private ILogger<UserGrain> _logger;
        private string _room = "";
        private string _name = "";

		public UserGrain(IClusterClient client, ILogger<UserGrain> logger, IHubContext<MyHub> hubContext)
		{
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask EnterChatRoom(string room)
        {
            if (_room != "") {
                await this.LeaveChatRoom();
            }
            _room = room;
            var roomGrain= _client.GetGrain<IChatRoom>(_room);
            await roomGrain.Join(this.GetPrimaryKeyString(), _name);
        }

        public async ValueTask LeaveChatRoom()
        {
            if (_room == "") return;
            var roomGrain = _client.GetGrain<IChatRoom>(_room);
            await roomGrain.Leave(this.GetPrimaryKeyString());
            _room = "";
        }

        public async ValueTask SendMessage(string message, DateTimeOffset time)
        {
            if (_room == "") return;
            var roomGrain = _client.GetGrain<IChatRoom>(_room);
            await roomGrain.SendMessage(this.GetPrimaryKeyString(), message, time);
        }

        public ValueTask SetName(string name)
        {
            _name = name;
            return ValueTask.CompletedTask;
        }

        public async ValueTask StartTyping()
        {
            if (_room == "") return;
            var roomGrain = _client.GetGrain<IChatRoom>(_room);
            await roomGrain.StartTyping(this.GetPrimaryKeyString());
        }

        public async ValueTask StoppedTyping()
        {
            if (_room == "") return;
            var roomGrain = _client.GetGrain<IChatRoom>(_room);
            await roomGrain.StopTyping(this.GetPrimaryKeyString());
        }
    }
}

