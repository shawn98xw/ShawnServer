using System;
namespace SceneServer
{
	public class Program
	{
		static void Main(string[] args)
        {
			//创建一个server
			SceneServer server = new SceneServer("aa", 8889);
			server.StartListen();
			while (true)
			{
				
			}
        }
	}
}