# 这是什么?
开发中的游戏服务器 Demo ShawnServer.sln；  
包含 2 类服务器进程 SceneServer、ChatServer。  
附简易命令行客户端 ShawnClient.sln。  

# 现在有什么功能了？
1. IOCP 模型实现的异步非阻塞的进程间通信功能。为了缓解内存碎片化，实现并使用了 Socket BufferPool，线程安全的 SAEA 对象池；

## 开发环境
Apple M1 Mac + MacOS 13.0 + Rider + .net 6.0


### 附 push 命令
```
git push git@github.com:shawn98xw/ShawnServer.git master
```

