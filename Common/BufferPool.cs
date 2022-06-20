using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Common;

/// <summary>
/// 缓存池
/// </summary>
public class BufferPool
{
    int totalSize; //缓存池总长度（Bytes）
    int buffSize; //单个缓存的长度（Bytes）
    int usedIndex; //缓存池从最小索引值开始使用，此变量记录曾经使用到的最大值
    Stack<int> IndexPool; //此栈记录缓存池中处于回收状态的缓存
    
    byte[] bufferBlock; //缓存池所在内存空间

    public BufferPool(int bufferCount, int buffSize)
    {
        this.totalSize = bufferCount * buffSize;
        this.buffSize = buffSize;
        usedIndex = 0;
        IndexPool = new Stack<int>();
        
        bufferBlock = new byte[totalSize];
    }
    //为作为参数传递进来的saes划分缓存空间
    public bool SetBuffer(SocketAsyncEventArgs saea)
    {
        if (IndexPool.Count > 0) //如果存在处于回收状态的缓存
        {   //从栈中取出缓存地址并赋予saea
            saea.SetBuffer(bufferBlock, IndexPool.Pop(), buffSize);
        }
        else //没有处于回收状态的缓存
        {   //如果缓存池空间不够则返回false
            if ((totalSize - buffSize) < usedIndex)
            {
                return false;
            }
            saea.SetBuffer(bufferBlock, usedIndex, buffSize);//分配缓存池中的新空间
            usedIndex += buffSize;//指定缓存池中新空间和旧空间的分界点
        }
        return true;
    }
    //释放saea所使用的缓存空间
    public void FreeBuffer(SocketAsyncEventArgs saea)
    {
        IndexPool.Push(saea.Offset);//将saea中用完的缓存地址压入栈中
        saea.SetBuffer(null, 0, 0);
    }
}