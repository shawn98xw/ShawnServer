using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SceneServer
{
	public class Role
	{
		public int x;
		public int y;

		public Role()
		{
			this.x = 0;
			this.y = 0;
		}
	}
	public class SceneServer
	{
		public string ServerName;
		public string ip;
		public int port;
		private Socket listenSocket;
		private byte[] buffer;
		private Dictionary<Socket, Role> rolesMap;
		public SceneServer(string name, int port)
		{
			this.ServerName = name;
			ip = "0.0.0.0";
			this.port = port;
			listenSocket = null;
			
			buffer = new byte[1024 * 1024 * 2];
			rolesMap = new Dictionary<Socket, Role>();
			(int id, int liveness) newLeader = (0, 0);
		}
		public void StartListen()
		{
			//1 实例化套接字(IP4寻找协议,流式协议,TCP协议)
			listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//2 创建IP对象
			IPAddress address = IPAddress.Parse(ip);
			//3 创建网络端口,包括ip和端口
			IPEndPoint endPoint = new IPEndPoint(address, port);
			//4 绑定套接字
			listenSocket.Bind(endPoint);
			//5 设置最大连接数
			listenSocket.Listen(int.MaxValue);
			Console.WriteLine($"监听端口：{listenSocket.LocalEndPoint}");
			//6 异步等待客户端连接
			var acceptEventArgs = createAcceptEventArgs();
			bool res = listenSocket.AcceptAsync(acceptEventArgs);
		}
		private SocketAsyncEventArgs createAcceptEventArgs()
		{
			var acceptEventArgs = new SocketAsyncEventArgs();
			acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(accept_Complete);
			return acceptEventArgs;
		}
		private void accept_Complete(object obj, SocketAsyncEventArgs acceptEventArgs)
		{
			//接收客户端消息
			var ioEventArgs = createIOEventArgs();
			ioEventArgs.AcceptSocket = acceptEventArgs.AcceptSocket;
			ioEventArgs.AcceptSocket.ReceiveAsync(ioEventArgs);
		}

		private SocketAsyncEventArgs createIOEventArgs()
		{
			var ioEventArgs = new SocketAsyncEventArgs();
			ioEventArgs.SetBuffer(new byte[1024 * 1024 * 2]);
			ioEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(io_Complete);
			return ioEventArgs;
		}

		private void io_Complete(object obj, SocketAsyncEventArgs ioEventArgs)
		{
			string str = Encoding.UTF8.GetString(ioEventArgs.Buffer);
			Console.WriteLine(str);
		}

		/// <summary>
		/// 监听客户端连接
		/// </summary>
		private void ListenClientConnect()
		{
			while (true)
			{
				//阻塞地等待客户端连接，处理客户端链接业务
				Socket clientSocket = listenSocket.Accept();
				Role role = new Role();
				rolesMap.Add(clientSocket, role);
				clientSocket.Send(Encoding.UTF8.GetBytes($"Connected to server {listenSocket.LocalEndPoint}"));
				var task = new Task(() =>
				{
					ReceiveMessage(clientSocket);
				});
				task.Start();
			}
		}
		/// <summary>
		/// 接收客户端消息
		/// </summary>
		/// <param name="socket">来自客户端的socket</param>
		private void ReceiveMessage(object socket)
		{
			Socket clientSocket = (Socket)socket;
			while (true)
			{
				int length = clientSocket.Receive(buffer);
				if (length == 0)
				{
					rolesMap.Remove(clientSocket);
					break;
				}

				if (false == rolesMap.TryGetValue(clientSocket, out Role role))
					continue;
				string str = Encoding.UTF8.GetString(buffer, 0, length);
				if (str == "l")
					role.x--;
				else if (str == "r")
					role.x++;
				else if (str == "u")
					role.y++;
				else if (str == "d")
					role.y--;
				else
				{
					
				}
				Console.WriteLine($"{clientSocket.RemoteEndPoint}:{str}, role position = ({role.x},{role.y})");
			}
		}
	}
}