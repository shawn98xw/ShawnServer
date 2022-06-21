using System;
using Common.Configs;
using Newtonsoft.Json;

namespace SceneServer
{
	public class Program
	{
		static void Main(string[] args)
		{
			//Config conf = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"./Config.json"));
			
			SceneServer server = new SceneServer();
			server.Net.Connect(8890); //连接ChatServer
			server.Net.Listen(8891); //监听客户端连接
			
			
			Console.ReadLine();
        }
	}
}