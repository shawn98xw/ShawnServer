using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Net;
 
namespace ShawnClient;
class Program
{
    static void Main(string[] args)
    {
        ShawnClient client = new ShawnClient();
        client.Net.Connect(8891); //连接SceneServer
        while (true)
        {
            Console.ReadLine();
            client.Net.Send("test1");
        }
        Console.ReadKey();
    }
}