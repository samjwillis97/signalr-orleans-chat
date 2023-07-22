using System;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Sockets.GrainInterfaces;
using Sockets.Hubs;

namespace Sockets.Grains
{
	public class ChatRoomGrain : Grain, IChatRoom
	{
        private readonly IHubContext<MyHub> _hubContext;
        private ILogger<ChatRoomGrain> _logger;
        private Dictionary<string, string> _userMap = new();
        private readonly string _roomId;

        private HashSet<string> _typingUsers = new();

		public ChatRoomGrain(ILogger<ChatRoomGrain> logger, IHubContext<MyHub> hubContext)
		{
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _roomId = $"chatroom-{this.GetPrimaryKeyString()}";
		}

        public async ValueTask Join(string id, string name)
        {
            _logger.LogInformation($"{name} entering - {_roomId}");

            if (_userMap.ContainsKey(id)) {
                await this.ChangeName(id, name);
                return;
	        }

            _userMap.Add(id, name);
            await _hubContext.Groups.AddToGroupAsync(id, _roomId);
            await _hubContext.Clients.Group(_roomId).SendAsync("userJoined", id, name, _userMap.Count);
        }

        public async ValueTask ChangeName(string id, string name) {
            var ok = _userMap.Remove(id, out var oldName);
            if (!ok) {
                throw new Exception("User does not exist");
            }
            _userMap.TryAdd(id, name);

            await _hubContext.Clients.Group(_roomId).SendAsync("userChangedName", id, oldName, name);
        }

        public async ValueTask Leave(string id)
        {
            var ok = _userMap.Remove(id, out var name);
            if (!ok) {
                throw new Exception("Unable to remove user");
            }

            if (_typingUsers.Contains(id)) {
                await this.StopTyping(id);
            }

            await _hubContext.Groups.RemoveFromGroupAsync(id, _roomId);
            await _hubContext.Clients.Group(_roomId).SendAsync("userLeft", id, name, _userMap.Count);
        }

        public async ValueTask SendMessage(string id, string message, DateTimeOffset time)
        {
            var ok = _userMap.TryGetValue(id, out var name);
            if (!ok) throw new Exception("User should not be in room");

            _logger.LogInformation($"{name}: {message}");
            await _hubContext.Clients.Group(_roomId).SendAsync("messageSent", id, name, message, time);
        }

        public async ValueTask StartTyping(string id)
        {
            _typingUsers.Add(id);
            await this.EmitTypingUsers();
        }

        public async ValueTask StopTyping(string id)
        {
            _typingUsers.Remove(id);
            await this.EmitTypingUsers();
        }

        private async ValueTask EmitTypingUsers() {
            var typingUserNames = new List<string>();
            foreach (var user in _typingUsers) {
                var ok = _userMap.TryGetValue(user, out var name);
                if (!ok) throw new Exception("Typing user does not exist");
                typingUserNames.Add(name);
            }

            await _hubContext.Clients.Group(_roomId).SendAsync("usersTyping", typingUserNames);
        }
    }
}

