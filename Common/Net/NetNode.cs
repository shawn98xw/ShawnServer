using System.Net.Sockets;
using System.Security.Cryptography;

namespace Common.Net;

public enum EServiceType
{
    Client = 0,
    Scene = 1,
    Chat = 2,
}
public enum EnumConnectionEventType
{
    accept,
    connect,
    disconnect,
    receive,
    close,
}
public class ConnectionEventArgs : EventArgs
{
    public EnumConnectionEventType EventType { get; set; }
    public Socket EventSocket { get; set; }
    public byte[] Buffer { get; set; }
    public ConnectionEventArgs(EnumConnectionEventType eventType, Socket socket)
    {
        EventType = eventType;
        EventSocket = socket;
        Buffer = new byte[1024];
    }
} 
public class NetNode
{
    public EServiceType ServiceType;
    public NetWork Net;
    public Socket ConnSocket;
    public List<ExSocket> ListenSockets;

    public NetNode(EServiceType serviceType)
    {
        ServiceType = serviceType;
        Net = new NetWork();
        ListenSockets = new List<ExSocket>();
    }

    public void HandleEvents()
    {
        int count = Net.ArgsQueue.Count;
        for(int i = 0; i < count; i++)
        {
            if(Net.ArgsQueue.TryDequeue(out var args) == false)
                continue;
            switch (args.EventType)
            {
                case EnumConnectionEventType.accept:
                    onAccept(args);
                    break;
                case EnumConnectionEventType.connect:
                    onConnect(args);
                    break;
                case EnumConnectionEventType.disconnect:
                    onDisconnect(args);
                    break;
                case EnumConnectionEventType.receive:
                    onReceive(args);
                    break;
                case EnumConnectionEventType.close:
                    onClose(args);
                    break;
            }
        }
    }

    private void onAccept(ConnectionEventArgs args)
    {
        ListenSockets.Add(args.EventSocket);
    }
    private void onConnect(ConnectionEventArgs args)
    {
        ConnSocket = args.EventSocket;
    }
    private void onDisconnect(ConnectionEventArgs args)
    {
        ConnSocket = null;
    }
    
    public virtual void onReceive(ConnectionEventArgs args)
    {
        
    }
    private void onClose(ConnectionEventArgs args)
    {
        ListenSockets.Clear();
    }
}