using System.Net.Sockets;

namespace Common.Net;

public class ExSocket
{
    public Socket Socket { get; set; }
    public EServiceType ServiceType { get; set; }
    ExSocket(Socket socket, EServiceType serviceType)
    {
        Socket = socket;
        ServiceType = serviceType;
    }
}