using System;
namespace SceneServer
{
	public class Program
	{
		static void Main(string[] args)
        {
			SceneServer server = new SceneServer();
			server.Net.Listen(8890);
			
			Console.ReadLine();
        }
	}
}