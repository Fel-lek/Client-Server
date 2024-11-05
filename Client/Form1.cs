using System.Net;
using System.Net.Sockets;

namespace Client
{
    public partial class Login : Form
    {
        public Socket Socket { get; private set; }
        IPEndPoint _serverEndPoint;
        private byte[] buffer;
        public Protocol clientProtocol;

        public Login(Socket socket, IPEndPoint serverEndPoint)
        {
            InitializeComponent();

            Socket = socket;
            _serverEndPoint = serverEndPoint;
            buffer = new byte[1024];
        }

        public void SubmitButton_Click(object sender, EventArgs e)
        {
            CheckLogin();
        }

        public void CheckLogin()
        {
            try
            {
                Socket.Connect(_serverEndPoint);

                if (Socket.Connected)
                {
                    clientProtocol = new Protocol(Socket, buffer, this);

                    clientProtocol.SendPacket(new LoginPacket()
                    {
                        Username = maskedTextBox1.Text,
                        Password = maskedTextBox2.Text
                    });

                    Thread receiveThread = new Thread(clientProtocol.ReceivePackets)
                    {
                        IsBackground = true
                    };
                    receiveThread.Start();

                    //byte[] readBuffer = new byte[1024];

                    //PacketReader reader = new PacketReader(readBuffer);

                    //Socket.Receive(readBuffer);

                    //Console.WriteLine("received");

                    //string messageIN = reader.ReadString();

                    //if (messageIN.Equals(""))
                    //{
                    //    Console.WriteLine("empty string");
                    //    continue;
                    //}
                    //Console.WriteLine(reader.ReadString());
                    //if (String.Compare(messageIN, "Invalid Login") == 0)
                    //{
                    //    Socket.Disconnect(true);
                    //    Console.WriteLine("Reenter Login");
                    //}
                    //if (messageIN.Contains($"Confirmation code sent to"))
                    //{
                    //    Console.WriteLine(messageIN);
                    //    //Task.Run(BeginReceiveMessages);
                    //    //SendMessage();
                    //}
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //public void ReceivePacket()
        //{
        //    var socket = this.Socket;
        //    var buffer = this.buffer;
        //    try
        //    {
        //        while (Socket.Connected)
        //        {
        //            int totalRead = 0;
        //            while (totalRead < 5)
        //            {
        //                totalRead += socket.Receive(buffer, totalRead, buffer.Length - totalRead, 0);
        //            }

        //            int size = BinaryPrimitives.ReadInt32LittleEndian(buffer);
        //            PacketType type = (PacketType)buffer[4];

        //            totalRead = 0;
        //            while (totalRead < size)
        //            {
        //                totalRead += socket.Receive(buffer, totalRead, size - totalRead, 0);
        //            }

        //            HandlePacket(size, type);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    //finally
        //    //{
        //    //    Disconnect();
        //    //}
        //}
    }
}