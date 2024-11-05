using System.Net;
using System.Net.Sockets;

namespace Client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 3500);

            ApplicationConfiguration.Initialize();
            Login loginForm = new Login(socket, serverEndPoint);
            Application.Run(loginForm);
            //for (int i = 0; i < 1; i++)
            //{
            //    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("86.6.9.31"), 3500);


            //    byte[] buffer = new byte[2048];

            //    writer = new PacketWriter(buffer);

            //    //buffer = Encoding.UTF8.GetBytes(message);

            //    //buffer = writer.WriteString();


            //}
        }
    }
}