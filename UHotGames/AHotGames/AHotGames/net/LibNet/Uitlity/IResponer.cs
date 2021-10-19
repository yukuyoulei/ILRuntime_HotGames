using LibPacket;
public interface IResponer
{
	void Response(PktBase message);
	string playerConnDesc { get; }
}
