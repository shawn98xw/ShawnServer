using System;
namespace SServer
{
	public class Program
	{
		public Program()
		{
		}

		static void Main(string[] args)
        {
			//创建一个server
			Server server = new Server("aa", 8888);
			//Console.WriteLine($"hi, {server.Name}");
			server.Start();
		}
	}
}