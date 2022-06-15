using SocketUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace SocketClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketClient client = new SocketClient(8888);
            client.StartClient();
            Console.ReadKey();
        }
    }
}