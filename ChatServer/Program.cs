using System;
namespace ChatServer;

public class Program
{
    public Program()
    {
    }

    static void Main(string[] args)
    {
        ChatServer chatServer = new ChatServer("🎒", 2999);
        chatServer.Start();
    }
}