using static Client;

public abstract class IncomingPacket
{
    public abstract PacketType PacketType { get; }

    public abstract void Deserialize(PacketReader reader);
}

public class LoginPacket : IncomingPacket
{
    public override PacketType PacketType => PacketType.Login;
    public string Username { get; private set; }
    public string Password { get; private set; }

    public override void Deserialize(PacketReader reader)
    {
        Username = reader.ReadString();
        Password = reader.ReadString();
    }
}

public class MessagePacket : IncomingPacket
{
    public override PacketType PacketType => PacketType.Message;

    public string Message { get; private set; }

    public override void Deserialize(PacketReader reader)
    {
        Message = reader.ReadString();
    }
}

public abstract class OutgoingPacket
{
    public abstract PacketType PacketType { get; }
    public abstract void Serialize(ResizableWriter writer);
}

public class LoginResponse : OutgoingPacket
{
    public override PacketType PacketType => PacketType.Login;

    public bool Response;

    public override void Serialize(ResizableWriter writer)
    {
        writer.Write(Response);
    }
}

public class ForwardMessage : OutgoingPacket
{
    public override PacketType PacketType => PacketType.Message;

    public string Message;

    public override void Serialize(ResizableWriter writer)
    {
        writer.Write(Message);
    }
}