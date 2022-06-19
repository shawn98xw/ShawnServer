using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

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
			//实例化套接字(IP4寻找协议,流式协议,TCP协议)
			listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//创建IP对象
			IPAddress address = IPAddress.Parse(ip);
			//创建网络端口,包括ip和端口
			IPEndPoint endPoint = new IPEndPoint(address, port);
			//绑定套接字
			listenSocket.Bind(endPoint);
			//设置最大连接数
			listenSocket.Listen(int.MaxValue);
			Console.WriteLine($"监听端口：{listenSocket.LocalEndPoint}");
			
			//异步等待客户端连接
			var acceptEventArgs = new SocketAsyncEventArgs();
			acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(acceptComplete);
			startAccept(acceptEventArgs);
		}

		private void startAccept(SocketAsyncEventArgs e)
		{
			e.AcceptSocket = null;
			if (listenSocket.AcceptAsync(e) == false) //异步侦听连接
				acceptComplete(null, e);
		}
		private void acceptComplete(object obj, SocketAsyncEventArgs e)
		{
			Socket clientSocket = e.AcceptSocket;
			Console.WriteLine($"侦听到来自{clientSocket.RemoteEndPoint}的连接请求");

			//开启一个新线程异步读取消息
			SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
			receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(receiveComplete);
			receiveArgs.SetBuffer(new byte[1024], 0, 1024); //设置读取缓存
			receiveArgs.AcceptSocket = e.AcceptSocket;
			startReceive(receiveArgs);

			//开启一个新线程异步发送消息
			SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
			sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(sendComplete);
			sendArgs.AcceptSocket = e.AcceptSocket;
			sendArgs.UserToken = 1; //用户数据
			startSend(sendArgs);
			
			startAccept(e); //进入下一个侦听周期
		}

		private void startReceive(SocketAsyncEventArgs e)
		{
			if (e.AcceptSocket.ReceiveAsync(e) == false)
				receiveComplete(null, e);
		}
		private void receiveComplete(object obj, SocketAsyncEventArgs e)
		{
			if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success)
			{
				Console.WriteLine($"关闭来自{e.AcceptSocket.RemoteEndPoint}的连接");
				e.AcceptSocket.Shutdown(SocketShutdown.Both);
				e.AcceptSocket.Close();
				return;
			}
			string str = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
			Console.WriteLine($"收到来自{e.AcceptSocket.RemoteEndPoint}的消息：{str}");
			
			//进入下一个读取周期
			startReceive(e);
		}

		private void startSend(SocketAsyncEventArgs e)
		{
			if ((int)e.UserToken > 5)
				return;

			byte[] sendBuff = Encoding.UTF8.GetBytes($"服务器消息：{e.UserToken}");
			e.SetBuffer(sendBuff, 0, sendBuff.Length);

			if(e.AcceptSocket.SendAsync(e) == false)
				sendComplete(null, e);
		}
		private void sendComplete(object obj, SocketAsyncEventArgs e)
		{
			if (e.SocketError != SocketError.Success)
			{
				e.AcceptSocket.Shutdown(SocketShutdown.Send);
				e.AcceptSocket.Close();
				return;
			}
			Console.WriteLine($"成功向{e.RemoteEndPoint}发送信息：{e.UserToken}");
			e.UserToken = (int)e.UserToken + 1;
			Thread.Sleep(500);
			startSend(e);
		}
	}
}