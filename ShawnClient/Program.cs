using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace ShawnClient;
class Program
{
    static void Main(string[] args)
    {
        ShawnClient client = new ShawnClient();
        client.Net.Connect(8890);
        Console.ReadKey();
    }
}