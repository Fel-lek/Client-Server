using Client;
using System.Buffers.Binary;
using System.Net.Sockets;

public abstract class IncomingPacket
{
    public abstract void Deserialize(PacketReader reader);
}
public class LoginResponse : IncomingPacket
{
    public bool Response;

    public override void Deserialize(PacketReader reader)
    {
        Response = reader.ReadBool();
    }
}

public class MessageIn : IncomingPacket
{
    public string Message;

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

public class LoginPacket : OutgoingPacket
{
    public override PacketType PacketType => PacketType.Login;

    public string Username;
    public string Password;

    public override void Serialize(ResizableWriter writer)
    {
        writer.Write(Username);
        writer.Write(Password);
    }
}

public class MessagePacket : OutgoingPacket
{
    public override PacketType PacketType => PacketType.Message;

    public string Message { get; set; }

    public override void Serialize(ResizableWriter writer)
    {
        writer.Write(Message);
    }
}

public class Protocol
{
    private byte[] buffer;
    public Socket Socket { get; private set; }
    public Login loginForm;

    public Protocol(Socket client, byte[] buffer, Login loginForm)
    {
        Socket = client;
        this.buffer = buffer;
        this.loginForm = loginForm;
    }

    public void ReceivePackets()
    {
        var socket = this.Socket;
        var buffer = this.buffer;
        try
        {
            while (Socket.Connected)
            {
                int totalRead = 0;
                while (totalRead < 5)
                {
                    totalRead += socket.Receive(buffer, totalRead, 5, 0);
                }

                int size = BinaryPrimitives.ReadInt32LittleEndian(buffer);
                PacketType type = (PacketType)buffer[4];

                if ((uint)size >= buffer.Length)
                {
                    throw new Exception("Buffer overflow");
                }

                totalRead = 0;
                while (totalRead < size)
                {
                    totalRead += socket.Receive(buffer, totalRead, size - totalRead, 0);
                }

                HandlePacket(size, type);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void Disconnect()
    {
        Socket.Shutdown(SocketShutdown.Both);
        Socket.Close();
    }

    public void HandlePacket(int size, PacketType type)
    {
        PacketReader reader = new PacketReader(buffer);

        switch (type)
        {
            case PacketType.Login:
                var login = new LoginResponse();
                login.Deserialize(reader);
                OnLoginResponse(login);
                break;
            case PacketType.Message:
                var message = new MessageIn();
                message.Deserialize(reader);
                OnMessage(message);
                break;
            default:
                break;
        }
    }

    public void SendPacket(OutgoingPacket packet)
    {
        ResizableWriter writer = new ResizableWriter();
        writer._position = 4;
        writer.Write((byte)packet.PacketType);
        packet.Serialize(writer);
        BinaryPrimitives.WriteInt32LittleEndian(writer._buffer, writer._position - 5);
        //Console.WriteLine(string.Join(",", writer._buffer));
        Socket.Send(writer._buffer, 0, writer._position, 0);
    }

    public void OnLoginResponse(LoginResponse login)
    {
        if (!login.Response)
            return;

        Console.WriteLine("PASS");
        Main mainForm = new Main(loginForm.clientProtocol);
        mainForm.ShowDialog();
        loginForm.Close();
    }

    public void SendMessage()
    {
        while (true)
        {
            SendPacket(new MessagePacket()
            {
                Message = Console.ReadLine()
            });
        }
    }

    public void OnMessage(MessageIn message)
    {
        Console.WriteLine(message.Message);
    }
}

public enum PacketType : byte
{
    Login,
    Message
}