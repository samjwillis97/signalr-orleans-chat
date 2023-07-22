namespace Sockets.GrainInterfaces;

public interface IUser : IGrainWithStringKey
{
    ValueTask SetName(string name);
    ValueTask EnterChatRoom(string room);
    ValueTask LeaveChatRoom();
    ValueTask SendMessage(string message, DateTimeOffset time);
    ValueTask StartTyping();
    ValueTask StoppedTyping();
}
