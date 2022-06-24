using System;

namespace SceneServer
{
	public class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine($"args invalid");
				return;
			}
			
			SceneServer server = new SceneServer();
			server.Node.Net.Listen(int.Parse(args[0])); //监听客户端连接
			server.Node.Net.Connect(8890); //连接ChatServer
			
			Console.ReadLine();
        }
	}
}