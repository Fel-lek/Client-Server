using System.Net;
using System.Net.Sockets;

public class PortForwarder
{
    IPEndPoint localEndPoint;
    EndPoint remoteEndPoint;
    Socket socket;

    public PortForwarder(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, Socket socket, int port)
    {
        this.localEndPoint = localEndPoint;
        this.remoteEndPoint = remoteEndPoint;
        this.socket = socket;
    }
}