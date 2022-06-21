using System;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common.Net;

namespace ChatServer;

public class ChatServer
{
    public NetWork Net;

    public ChatServer()
    {
        Net = new NetWork();
    }
}