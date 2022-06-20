using System;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common;

namespace SceneServer;

public class SceneServer
{
	public string ServerName;
	public string ip;
	public int port;
	
	public SceneServer(string name, int port)
	{
		this.ServerName = name;
		ip = "0.0.0.0";
		this.port = port;
	}
}
