using System;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common.Net;

namespace ShawnClient;

public class ShawnClient
{
    public NetWork Net;

    public ShawnClient()
    {
        Net = new NetWork();
    }
}