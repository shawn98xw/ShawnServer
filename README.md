## Socket异步编程模式
### 特点
异步非阻塞的 Socket 通信功能；  
使用 SocketAsyncEventArgs 类，SAEA 类中实现了 IOCP 模型；  
加入 Buffer 缓存池；  
加入线程安全的 SAEA 对象池。
### 相关概念
#### 异步通信
![image](https://github.com/shawn98xw/ShawnServer/blob/master/READMEIMG/async_img.png)
#### IOCP模型
![image](https://github.com/shawn98xw/ShawnServer/blob/master/READMEIMG/iocp_img.png)


