using System.Net.Sockets;

public class Connection
{
    Socket clientSocket;

    public Connection(Socket client)
    {
        this.clientSocket = client;
    }
}