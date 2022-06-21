using System;
using Common.Configs;
using Newtonsoft.Json;

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
			//Config conf = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"./Config.json"));
			
			SceneServer server = new SceneServer();
			server.Net.Listen(int.Parse(args[0])); //监听客户端连接
			server.Net.Connect(8890); //连接ChatServer
			
			
			
			Console.ReadLine();
        }
	}
}