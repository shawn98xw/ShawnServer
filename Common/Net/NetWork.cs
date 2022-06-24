using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Resources;
using System.Text;

namespace Common.Net;

/// <summary>
/// 参考IOCP模型实现的Socket通信功能
/// </summary>
public class NetWork
{
	private Socket listenSocket;
	private SocketAsyncEventArgsPool saeaPool; //saea对象池
	private BufferPool bufferPool; //buffer缓存池
	
	private int connectCount; //最大连接数
	private int bufferSize; //缓存单位
	private SemaphoreSlim acceptLimit; //控制同时访问线程数的信号量
	public ConcurrentQueue<ConnectionEventArgs> ArgsQueue;
	public NetWork()
	{
		listenSocket = null;
		connectCount = 20;
		bufferSize = 25;

		bufferPool = new BufferPool(connectCount * bufferSize, bufferSize);
		saeaPool = new SocketAsyncEventArgsPool(connectCount);
		ArgsQueue = new ConcurrentQueue<ConnectionEventArgs>();
		for (int i = 0; i < connectCount; i++)
		{
			SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
			saea.Completed += new EventHandler<SocketAsyncEventArgs>(IOCompleted);
			saeaPool.Push(saea);
		}

		acceptLimit = new SemaphoreSlim(connectCount, connectCount);
	}

	#region Listen & Accept
	/// <summary>
	/// 作为服务器开始监听客户端连接
	/// </summary>
	/// <param name="port"></param>
	public void Listen(int port)
	{
		//实例化套接字(IP4寻找协议,流式协议,TCP协议)
		listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		//创建IP对象
		IPAddress address = IPAddress.Parse("0.0.0.0");
		//创建网络端口,包括ip和端口
		IPEndPoint endPoint = new IPEndPoint(address, port);
		//绑定套接字
		listenSocket.Bind(endPoint);
		//侦听队列：设置最大连接数
		listenSocket.Listen(connectCount);
		Console.WriteLine($"监听端口：{listenSocket.LocalEndPoint}");
		
		//异步等待客户端连接
		var acceptEventArgs = new SocketAsyncEventArgs();
		acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(acceptCompleted);
		startAccept(acceptEventArgs);
	}
	private void startAccept(SocketAsyncEventArgs e)
	{
		acceptLimit.Wait();
		
		e.AcceptSocket = null;
		if (listenSocket.AcceptAsync(e) == false) //异步侦听连接
			acceptCompleted(null, e);
	}
	private void acceptCompleted(object obj, SocketAsyncEventArgs e)
	{
		Socket conn = e.AcceptSocket;
		//conn.LocalEndPoint
		Console.WriteLine($"侦听到来自{conn.RemoteEndPoint}的连接请求");
		//开启一个新线程异步接收消息
		SocketAsyncEventArgs saea = saeaPool.Pop();
		bufferPool.SetBuffer(saea);
		saea.AcceptSocket = e.AcceptSocket;
		startReceive(saea);

		startAccept(e); //进入下一个侦听周期
	}
	#endregion

	#region Connect
	/// <summary>
	/// 作为客户端发起连接
	/// </summary>
	public void Connect(int port)
	{
		Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		var connectEventArgs = new SocketAsyncEventArgs();
		connectEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(connectCompleted);
		connectEventArgs.AcceptSocket = clientSocket;
		IPAddress address = IPAddress.Parse("127.0.0.1");
		connectEventArgs.RemoteEndPoint = new IPEndPoint(address, port);

		if (clientSocket.ConnectAsync(connectEventArgs) == false)
			connectCompleted(null, connectEventArgs);
	}
	private void connectCompleted(object obj, SocketAsyncEventArgs e)
	{
		if(e.SocketError != SocketError.Success)
			return;

		Socket clientSocket = e.AcceptSocket;
		Console.WriteLine($"连接到{clientSocket.RemoteEndPoint}");

		//开启一个新线程异步接收消息
		SocketAsyncEventArgs saea = saeaPool.Pop();
		bufferPool.SetBuffer(saea);
		saea.AcceptSocket = e.AcceptSocket;
		startReceive(saea);
	}
	#endregion
	private void startReceive(SocketAsyncEventArgs e)
	{
		if (e.AcceptSocket.ReceiveAsync(e) == false)
			receiveCompleted(null, e);
	}
	private void receiveCompleted(object obj, SocketAsyncEventArgs e)
	{
		if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success)
		{
			closeSocket(e);
			return;
		}
		string str = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
		Console.WriteLine($"收到来自{e.AcceptSocket.RemoteEndPoint}的消息：{str}");
		
		e.SetBuffer(0, bufferSize);
		startReceive(e); //进入下一个接收周期
	}

	public void Send(Socket conn, string str)
	{
		if(conn == null)
			return;
		
		byte[] data = Encoding.UTF8.GetBytes(str);
		var saea = saeaPool.Pop();
		bufferPool.SetBuffer(saea);
		saea.SetBuffer(data);
		saea.AcceptSocket = conn;
		startSend(saea);
	}
	private void startSend(SocketAsyncEventArgs e)
	{
		if(e.AcceptSocket.SendAsync(e) == false)
			sendCompleted(null, e);
	}
	private void sendCompleted(object obj, SocketAsyncEventArgs e)
	{
		if (e.SocketError != SocketError.Success)
		{
			closeSocket(e);
			return;
		}
		
		bufferPool.FreeBuffer(e); //释放缓存
		saeaPool.Push(e); //回收saea进对象池
	}

	private void IOCompleted(object obj, SocketAsyncEventArgs e)
	{
		switch (e.LastOperation)
		{
			case SocketAsyncOperation.Receive:
				receiveCompleted(null, e);
				break;
			case SocketAsyncOperation.Send:
				sendCompleted(null, e);
				break;
			default:
				return;
		}
	}

	private void closeSocket(SocketAsyncEventArgs e)
	{
		string str = e.AcceptSocket.RemoteEndPoint.ToString();
		
		e.AcceptSocket.Shutdown(SocketShutdown.Send);
		e.AcceptSocket.Close();
		acceptLimit.Release(); //释放信号量
		bufferPool.FreeBuffer(e); //释放缓存
		saeaPool.Push(e); //回收saea进对象池
		Console.WriteLine($"关闭来自{str}的连接");
	}
}