using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SServer
{
	public class Server : IServer
	{
		public string name;
		public string ip;
		public int port;
		private Socket socket;
		private byte[] buffer;
		public Server(string name, int port)
		{
			this.name = name;
			ip = "0.0.0.0";
			this.port = port;
			socket = null;
			buffer = new byte[1024 * 1024 * 2];
		}
		public void Start()
		{
			//1 实例化套接字(IP4寻找协议,流式协议,TCP协议)
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//2 创建IP对象
			IPAddress address = IPAddress.Parse(ip);
			//3 创建网络端口,包括ip和端口
			IPEndPoint endPoint = new IPEndPoint(address, port);
			//4 绑定套接字
			socket.Bind(endPoint);
			//5 设置最大连接数
			socket.Listen(int.MaxValue);
			Console.WriteLine($"监听端口：{socket.LocalEndPoint}");
			//6 开始监听
			var task = new Task(() =>
			{
				ListenClientConnect();
			});
			task.Start();
			task.Wait();
		}
		public void Stop()
		{

		}
		public void ServerRun()
		{
			Start();
		}
		/// <summary>
		/// 监听客户端连接
		/// </summary>
		private void ListenClientConnect()
		{
			while (true)
			{
				//阻塞地等待客户端连接，处理客户端链接业务
				Socket clientSocket = socket.Accept();
				clientSocket.Send(Encoding.UTF8.GetBytes($"Connected to server {socket.LocalEndPoint}"));
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
					break;
				Console.WriteLine($"{clientSocket.RemoteEndPoint}:{Encoding.UTF8.GetString(buffer, 0, length)}");
			}
		}
	}
}