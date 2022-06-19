# 这是什么?
这是一个正在开发中的游戏服务器 Demo ShawnServer.sln；  
其中包含 2 种进程 SceneServer、ChatServer。  
此外带有一个简易的命令行客户端 ShawnClient.sln。  

# 现在有什么功能了？
1. 采用 IOCP 模型实现的异步非阻塞的进程间通信。为了缓解内存碎片化，实现并使用了Socket BufferPool，线程安全的 SAEA 对象池；

## 开发环境
Apple M1 Mac + MacOS 13.0 + Rider + .net 6.0


### 附 push 命令
```
git push git@github.com:shawn98xw/ShawnServer.git master
```