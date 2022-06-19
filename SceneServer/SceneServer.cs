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
	private Socket listenSocket;
	private SocketAsyncEventArgsPool saeaPool; //saea对象池
	private BufferPool bufferManager; //buffer缓存池
	private int connectCount; //最大连接数
	private int bufferSize; //缓存单位
	private SemaphoreSlim acceptLimit; //控制同时访问线程数的信号量
	public SceneServer(string name, int port)
	{
		this.ServerName = name;
		ip = "0.0.0.0";
		this.port = port;
		listenSocket = null;
		connectCount = 20;
		bufferSize = 25;

		bufferManager = new BufferPool(connectCount * bufferSize, bufferSize);
		saeaPool = new SocketAsyncEventArgsPool(connectCount);
		
		for (int i = 0; i < connectCount; i++)
		{
			SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
			saea.Completed += new EventHandler<SocketAsyncEventArgs>(IOCompleted);
			saeaPool.Push(saea);
			
			//bufferManager.SetBuffer(saea);
		}

		acceptLimit = new SemaphoreSlim(connectCount, connectCount);
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
		acceptLimit.Wait();
		
		e.AcceptSocket = null;
		if (listenSocket.AcceptAsync(e) == false) //异步侦听连接
			acceptComplete(null, e);
	}
	private void acceptComplete(object obj, SocketAsyncEventArgs e)
	{
		Socket clientSocket = e.AcceptSocket;
		Console.WriteLine($"侦听到来自{clientSocket.RemoteEndPoint}的连接请求");

		//开启一个新线程异步读取消息
		SocketAsyncEventArgs saea = saeaPool.Pop();
		bufferManager.SetBuffer(saea);
		saea.AcceptSocket = e.AcceptSocket;
		startReceive(saea);

		startAccept(e); //进入下一个侦听周期
	}

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
		
		//将消息回发
		e.SetBuffer(e.Offset, e.BytesTransferred);
		startSend(e);
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
		
		e.SetBuffer(0, bufferSize);
		startReceive(e); //进入下一个接收周期
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
		bufferManager.FreeBuffer(e); //释放缓存
		saeaPool.Push(e); //回收saea进对象池
		Console.WriteLine($"关闭来自{str}的连接");
	}
}
