using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Collections.Generic;

namespace Common.Net;
/// <summary>
/// 加锁后支持多线程的saea对象池
/// </summary>
public class SocketAsyncEventArgsPool
{ 
    Stack<SocketAsyncEventArgs> saeaPool; //saea池

    public SocketAsyncEventArgsPool(int capacity)
    {
        saeaPool = new Stack<SocketAsyncEventArgs>(capacity);
    }

    public void Push(SocketAsyncEventArgs e)
    {
        if(e == null)
            return;

        lock (saeaPool)
        {
            saeaPool.Push(e);
        }
    }

    public SocketAsyncEventArgs Pop()
    {
        lock (saeaPool)
        {
            return saeaPool.Pop();
        }
    }
    
    public int Count
    {
        get { return saeaPool.Count; }
    }
}