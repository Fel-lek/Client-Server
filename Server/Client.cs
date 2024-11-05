using System.Buffers.Binary;
using System.Net.Sockets;

public class Client
{
    public Socket Socket { get; private set; }
    private Server server;
    private byte[] buffer;
    public string Username { get; private set; }
    private Dictionary<PacketType, Action> hooks;
    PacketReader reader;

    public Client(Socket socket, Server server)
    {
        this.Socket = socket;
        this.server = server;
        this.buffer = new byte[1024];
        reader = new PacketReader(buffer);
        AddHooks();
    }

    public async Task ReceivePackets()
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
                    int count = await socket.ReceiveAsync(new Memory<byte>(buffer, totalRead, 5 - totalRead));
                    if (count == 0)
                    {
                        throw new Exception("EOF");
                    }

                    totalRead += count;
                }
                //Console.WriteLine(string.Join(",", buffer));

                totalRead = 0;
                int size = BinaryPrimitives.ReadInt32LittleEndian(buffer);

                //Console.WriteLine(size);


                if ((uint)size >= buffer.Length)
                {
                    throw new Exception("Buffer overflow");
                }

                PacketType type = (PacketType)buffer[4];

                while (totalRead < size)
                {
                    int count = await socket.ReceiveAsync(new Memory<byte>(buffer, totalRead, size - totalRead));
                    if (count == 0)
                    {
                        throw new Exception("EOF");
                    }

                    totalRead += count;
                }

                HandlePacket(size, type);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void Disconnect()
    {
        Socket.Shutdown(SocketShutdown.Both);
        Socket.Close();
        server.clients.Remove(this);
    }

    private void HandlePacket(int size, PacketType type)
    {
        reader._position = 0;
        if (hooks.TryGetValue(type, out var hook))
        {
            hook();
        }

        //    //PacketReader reader = new PacketReader(buffer);

        //    //switch (type)
        //    //{
        //    //    case PacketType.Login:
        //    //        var login = new LoginPacket();
        //    //        login.Deserialize(reader);

        //    //        if (reader._position != size)
        //    //        {
        //    //            throw new Exception("buffer overflow attempt");
        //    //        }

        //    //        OnLoginPacket(login);
        //    //        break;
        //    //    case PacketType.Message:
        //    //        var message = new MessagePacket();
        //    //        message.Deserialize(reader);

        //    //        if (reader._position != size)
        //    //        {
        //    //            throw new Exception("buffer overflow attempt");
        //    //        }

        //    //        OnMessage(message);
        //    //        break;
        //    //    default:
        //    //        break;
        //    //}
    }

        private void OnLoginPacket(LoginPacket login)
    {
        if (!server.DataBase.GetUser(login.Username, login.Password))
        {
            //packetWriter.Write("Invalid Login");
            SendPacket(new LoginResponse()
            {
                Response = false
            });
            Socket.Disconnect(true);
            return;
        }

        //packetWriter = new ResizableWriter();
        //packetWriter.Write($"Confirmation code sent to {server.DataBase.GrabEmail(login.Username)}!");
        //Socket.Send(packetWriter._buffer);
        SendPacket(new LoginResponse()
        {
            Response = true
        });

        Username = login.Username;
        //Console.WriteLine(server.DataBase.GrabEmail(login.Username));
    }

    private void OnMessage(MessagePacket message)
    {
        Console.WriteLine(message.Message);
        string msg = message.Message.Insert(0, $"[{DateTime.Now}] {Username}: ");

        foreach (Client _client in server.clients)
        {
            if (_client == this)
                continue;

            //packetWriter = new ResizableWriter();
            //packetWriter.Write(message);

            _client.SendPacket(new ForwardMessage()
            {
                Message = msg
            });
        }
    }

    private void SendPacket(OutgoingPacket packet)
    {
        ResizableWriter writer = new ResizableWriter();
        writer._position = 4;
        writer.Write((byte)packet.PacketType);
        packet.Serialize(writer);
        BinaryPrimitives.WriteInt32LittleEndian(writer._buffer, writer._position - 5);
        Socket.Send(writer._buffer, 0, writer._position, 0);
    }

    public void Subscribe<T>(Action<T> action) where T : IncomingPacket, new()
    {
        T instance = new T();
        hooks.Add(instance.PacketType, () =>
        {
            instance.Deserialize(reader);
            action(instance);
        });
    }

    private void AddHooks()
    {
        hooks = new Dictionary<PacketType, Action>();

        Subscribe<LoginPacket>(OnLoginPacket);
        Subscribe<MessagePacket>(OnMessage);
    }
}

public enum PacketType : byte
{
    Login,
    Message
}