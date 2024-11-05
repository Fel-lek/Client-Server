using System.Net;
using System.Net.Sockets;

public class Server
{
    private List<Connection> pendingConnections;
    public HashSet<Client> clients;
    private Socket serverSocket;
    private IPEndPoint localIPEndPoint;
    IPEndPoint remoteIPEndPoint;
    private ushort availableClientIds;
    public DataBase DataBase;
    private PacketReader packetReader;
    private ResizableWriter packetWriter;

    private int maxConnections;
    private int port;
    private int maxPendingConnections;

    public Server(int maxConnections, int port, int maxPendingConnections)
    {
        this.maxConnections = maxConnections;
        this.port = port;
        this.maxPendingConnections = maxPendingConnections;

        pendingConnections = new List<Connection>();
        clients = new HashSet<Client>();
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Console.WriteLine(availableClientIds);

        OpenSocket(serverSocket);
    }

    private void OpenSocket(Socket socket)
    {
        localIPEndPoint = new IPEndPoint(IPAddress.Loopback, port);
        //remoteIPEndPoint = new IPEndPoint(IPAddress.Parse("86.6.9.31"), port);

        socket.Bind(localIPEndPoint);
        socket.Listen(maxPendingConnections);

        //Thread handleThread = new Thread(AcceptIncoming)
        //{
        //    IsBackground = true
        //};

        //handleThread.Start();
        DataBase = new DataBase();
        Console.WriteLine("The Server is open for connections");
        AcceptIncoming();
    }

    private void AcceptIncoming()
    {
        while (true)
        {
            try
            {
                Socket newClient = serverSocket.Accept();
                HandleNewConnection(newClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    private void HandleNewConnection(Socket client)
    {
        if (client.Connected)
        {
            ReceivePackets(client);
        }
    }
    public void ReceivePackets(Socket socket)
    {
        if (socket.Connected)
        {
            //packetReader = new PacketReader(HandleReceivePacket(socket));
            //string username = packetReader.ReadString();
            //string password = packetReader.ReadString();

            //if (!dataBase.GetUser(username, password)) //Values should be passed through Stream (TCP) from client (Fellek, passwrd)
            //{
            //    packetWriter = new ResizableWriter();
            //    packetWriter.Write("Invalid Login");
            //    socket.Send(packetWriter._buffer);
            //    Console.WriteLine(username);
            //    Console.WriteLine(password);

            //    //Console.WriteLine($"Disconnected client {client.RemoteEndPoint}:{port} due to failed login");
            //    //client.Disconnect(false);
            //    continue;
            //}

            //packetWriter = new ResizableWriter();
            //packetWriter.Write($"Confirmation code sent to {DataBase.GrabEmail(username)}!");
            //Console.WriteLine(DataBase.GrabEmail(username));
            //socket.Send(packetWriter._buffer);

            //Console.WriteLine(username);
            //Console.WriteLine(password);

            Client newClient = new Client(socket, this);
            clients.Add(newClient);
            Console.WriteLine($"Accepted new client from {newClient.Socket.RemoteEndPoint} on port {port} - Client name: ");
            Task.Run(newClient.ReceivePackets);

            //Task.Run(() => BeginReceiveMessages(newClient));
            //Console.WriteLine($"Disconnected new client from {client.RemoteEndPoint} on port {port}");
            //client.Disconnect(false);

            //HandlePacket(type, size, buffer, socket);
        }
    }

    private async void ListenForDisconnect(Client client)
    {

    }

    //public async Task BeginReceiveMessages(Client client)
    //{
    //    try
    //    {
    //        while (client.Socket.Connected)
    //        {
    //            //var buffer = new byte[1024];
    //            //client.Receive(buffer);
    //            client.ReceivePackets();

    //            foreach (Client _client in clients.Values)
    //            {
    //                if (_client == client)
    //                    continue;

    //                //packetWriter = new ResizableWriter();
    //                //packetWriter.Write(message);
    //                _client.Socket.Send(packetWriter._buffer);
    //            }
    //        }
    //    }
    //    catch (Exception ex) { Console.WriteLine(ex.Message); }
    //}
}

//public class TCPForwarder
//{
//    private readonly Socket _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

//    public void Start(IPEndPoint local, IPEndPoint remote, Socket client)
//    {
//        var destination = new TCPForwarder();
//        destination.Connect(remote, client);
//    }

//    private void Connect(EndPoint remoteEndpoint, Socket destination)
//    {
//        _mainSocket.Connect(remoteEndpoint);
//    }
//}