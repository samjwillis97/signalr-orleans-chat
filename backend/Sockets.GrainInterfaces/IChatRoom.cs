namespace Sockets.GrainInterfaces
{
	public interface IChatRoom : IGrainWithStringKey
	{
		ValueTask Join(string id, string name);
		ValueTask SendMessage(string id, string name, DateTimeOffset time);
        ValueTask ChangeName(string id, string name);
		ValueTask Leave(string id);
		ValueTask StartTyping(string id);
		ValueTask StopTyping(string id);
	}
}

