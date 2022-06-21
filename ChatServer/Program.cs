using System;
namespace ChatServer;

public class Program
{
    public Program()
    {
    }

    static void Main(string[] args)
    {
        ChatServer chatServer = new ChatServer();
        chatServer.Net.Listen(8890); //监听SceneServer连接

        Console.ReadLine();
    }
}