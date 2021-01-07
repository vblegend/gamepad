using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketServer.Net.Common;

namespace WebSocketServer.Net
{

    /// <summary>
    /// 带日志输出的 WebSocket 控制器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WebSocketController<T> : WebSocketController
    {
        /// <summary>
        /// 日志输出
        /// </summary>
        protected ILogger<T> logger { get; private set; }

        /// <summary>
        /// 构建一个带日志输出的 WebSocket控制器
        /// </summary>
        /// <param name="loggerfactory"></param>
        public WebSocketController(ILoggerFactory loggerfactory) : base()
        {
            logger = loggerfactory.CreateLogger<T>();
        }
    }




    /// <summary>
    /// WebSocket 控制器
    /// </summary>
    public abstract class WebSocketController
    {

        public WebSocketController()
        {
            Encoding = Encoding.UTF8;
            connections = new ConcurrentDictionary<HttpContext, ISocketContext>();
            ReceiveBufferSize = 65535;
        }

        #region Property
        /// <summary>
        /// 请求处理委托
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private delegate Task ReceiveProcessHandle(ISocketContext context);

        /// <summary>
        /// 编/解码字符集 默认为 UTF8 编码
        /// </summary>
        public Encoding Encoding { get; protected set; }

        /// <summary>
        /// 连接对象
        /// </summary>
        private ConcurrentDictionary<HttpContext, ISocketContext> connections { get; set; }

        /// <summary>
        /// 接收数据缓冲区大小  默认 65535
        /// </summary>
        protected Int32 ReceiveBufferSize { get; set; }
        #endregion

        /// <summary>
        /// 应答对象
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal async Task AcceptWebSocketAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                throw new Exception("该请求不是一个 WebSocket请求。");
            }
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            ISocketContext socketContext = new WebSocketClientContext(webSocket, this);
            ReceiveProcessHandle processHandle = new ReceiveProcessHandle(ReceiveProcess);
            connections.TryAdd(context, socketContext);
            try
            {
                await this.OnConnection(socketContext);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OnConnection error \r\n {ex.ToString()}");
            }
            await processHandle(socketContext);
            connections.TryRemove(context, out var removed);
        }



        private Dictionary<String, String> getRequestParams(String url)
        {
            var result = new Dictionary<String, String>();
            Int32 index = url.IndexOf("?");
            if (index > -1)
            {
                var str = url.Substring(index);
                var semgents = str.Split("&");
                for (var i = 0; i < semgents.Length; i++)
                {
                    var data = semgents[i].Split("=");
                    result[data[0]] = data[1];
                }
            }
            return result;
        }




        /// <summary>
        /// 请求接收数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task ReceiveProcess(ISocketContext context)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Byte[] buffer = new byte[ReceiveBufferSize];
                var result = await context.socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Binary:
                        case WebSocketMessageType.Text:
                            using (var write = new BinaryWriter(stream, this.Encoding, true))
                            {
                                write.Write(buffer, 0, result.Count);
                            }
                            if (result.EndOfMessage)
                            {
                                using (IDataBuffer dataBuffer = new DataBuffer(this, result.MessageType, stream))
                                {
                                    try
                                    {
                                        await this.OnReceive(context, dataBuffer);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"OnReceive error \r\n {ex.ToString()}");
                                    }
                                    stream.SetLength(0);
                                };
                            }
                            break;
                        case WebSocketMessageType.Close:
                            await context.socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                            await this.OnClose(context);
                            return;
                    }
                    result = await context.socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await context.socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                await this.OnClose(context);
            }
        }

        #region Methods

        /// <summary>
        /// 发送文本数据给所有客户端
        /// </summary>
        /// <param name="text">文本数据</param>
        /// <returns></returns>
        public async Task SendAll(String text)
        {
            var clients = new List<ISocketContext>(connections.Values);
            foreach (var client in clients)
            {
                if (client.IsConnect)
                {
                    await client.Send(text);
                }
            }
        }

        /// <summary>
        /// 发送二进制数据给所有客户端
        /// </summary>
        /// <param name="buffer">数据缓存</param>
        /// <returns></returns>
        public async Task SendAll(Byte[] buffer)
        {
            var clients = new List<ISocketContext>(connections.Values);
            foreach (var client in clients)
            {
                if (client.IsConnect)
                {
                    await client.Send(buffer);
                }
            }
        }

        /// <summary>
        /// 发送二进制数据给所有客户端
        /// </summary>
        /// <param name="buffer">数据缓存</param>
        /// <param name="offset">数据的开始位置</param>
        /// <param name="nCount">数据的长度</param>
        /// <returns></returns>
        public async Task SendAll(Byte[] buffer, Int32 offset, Int32 nCount)
        {
            var clients = new List<ISocketContext>(connections.Values);
            foreach (var client in clients)
            {
                if (client.IsConnect)
                {
                    await client.Send(buffer, offset, nCount);
                }
            }
        }


        /// <summary>
        /// 发送文本数据
        /// </summary>
        /// <param name="context">Socket上下文</param>
        /// <param name="text">发送的文本内容</param>
        /// <returns></returns>
        public async Task Send(ISocketContext context, String text)
        {
            await context.Send(text);
        }

        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="context">Socket上下文</param>
        /// <param name="buffer">数据</param>
        /// <returns></returns>
        public async Task Send(ISocketContext context, Byte[] buffer)
        {
            await context.Send(buffer);
        }

        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="context">Socket上下文</param>
        /// <param name="buffer">数组缓冲区</param>
        /// <param name="offset">数据的起始下标</param>
        /// <param name="nCount">数据长度</param>
        /// <returns></returns>
        public async Task Send(ISocketContext context, Byte[] buffer, Int32 offset, Int32 nCount)
        {
            await context.Send(buffer, offset, nCount);
        }

        /// <summary>
        /// 关闭一个Socket的链接
        /// </summary>
        /// <param name="context">socket上下文</param>
        /// <returns></returns>
        public async Task Close(ISocketContext context)
        {
            await Close(context, WebSocketCloseStatus.Empty, "");
        }

        /// <summary>
        /// 关闭一个Socket的链接
        /// </summary>
        /// <param name="context">socket上下文</param>
        /// <param name="status">指定关闭状态</param>
        /// <param name="description">指定关闭消息</param>
        /// <returns></returns>
        public async Task Close(ISocketContext context, WebSocketCloseStatus status, String description)
        {
            if (context.socket == null)
            {
                throw new Exception("socket is null");
            }
            if (context.socket.State == WebSocketState.Open)
            {
                await context.socket.CloseAsync(status, description, CancellationToken.None);
            }
        }
        #endregion

        /// <summary>
        /// 连接建立重写事件
        /// </summary>
        /// <param name="context">连接上下文</param>
        /// <returns></returns>
        public virtual async Task OnConnection(ISocketContext context)
        {
            await Task.Run(() =>
            {

            });
        }
        /// <summary>
        /// 连接关闭重写事件
        /// </summary>
        /// <param name="context">连接上下文</param>
        /// <returns></returns>
        public virtual async Task OnClose(ISocketContext context)
        {
            await Task.Run(() =>
            {

            });
        }
        /// <summary>
        /// 接收数据重写事件
        /// </summary>
        /// <param name="context">连接上下文</param>
        /// <param name="data">数据缓存，当前方法完成时该对象会销毁</param>
        /// <returns></returns>
        public virtual async Task OnReceive(ISocketContext context, IDataBuffer data)
        {
            await Task.Run(() =>
            {

            });
        }
    }
}
