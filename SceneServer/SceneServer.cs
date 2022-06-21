using System;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common.Net;

namespace SceneServer;

public class SceneServer
{
	public NetWork Net;

	public SceneServer()
	{
		Net = new NetWork();
	}
}
