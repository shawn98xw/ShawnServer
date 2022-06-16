using System;
namespace SceneServer
{
	public class Program
	{
		public Program()
		{
			
		}

		static void Main(string[] args)
        {
			//创建一个server
			SceneServer server = new SceneServer("aa", 8888);
			server.Start();
		}
	}
}