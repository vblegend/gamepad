using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketServer.Net.Common
{
    internal class WebSocketClientContext : ISocketContext
    {
        internal WebSocketClientContext(WebSocket _socket, WebSocketController _controller)
        {
            controller = _controller;
            valuePairs = new ConcurrentDictionary<string, object>();
            socket = _socket;
        }

        public WebSocket socket { get; private set; }

        public T Get<T>(string key)
        {
            if (valuePairs.TryGetValue(key, out Object value))
            {
                return (T)value;
            }
            return default(T);
        }

        public void Set<T>(string key, T value)
        {
            valuePairs.AddOrUpdate(key, value, (e, c) => { return value; });
        }



        #region Send

        /// <summary>
        /// 发送文本数据
        /// </summary>
        /// <param name="context">Socket上下文</param>
        /// <param name="text">发送的文本内容</param>
        /// <returns></returns>
        public async Task Send(String text)
        {
            if (this.socket.State != WebSocketState.Open)
            {
                throw new Exception("当前连接已断开");
            }
            var buffer = Encoding.GetBytes(text);
            var sendbuffer = new ArraySegment<byte>(buffer, 0, buffer.Length);
            await this.socket.SendAsync(sendbuffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }


        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="context">Socket上下文</param>
        /// <param name="buffer">数据</param>
        /// <returns></returns>
        public async Task Send(Byte[] buffer)
        {
            if (buffer == null)
            {
                throw new Exception("参数 buffer 不能为null");
            }

            if (this.socket.State != WebSocketState.Open)
            {
                throw new Exception("当前连接已断开");
            }
            var sendbuffer = new ArraySegment<byte>(buffer, 0, buffer.Length);
            await this.socket.SendAsync(sendbuffer, WebSocketMessageType.Binary, true, CancellationToken.None);
        }


        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="context">Socket上下文</param>
        /// <param name="buffer">数组缓冲区</param>
        /// <param name="offset">数据的起始下标</param>
        /// <param name="nCount">数据长度</param>
        /// <returns></returns>
        public async Task Send(Byte[] buffer, Int32 offset, Int32 nCount)
        {
            if (buffer == null)
            {
                throw new Exception("参数 buffer 不能为null");
            }
            if (this.socket.State != WebSocketState.Open)
            {
                throw new Exception("当前连接已断开");
            }
            var sendbuffer = new ArraySegment<byte>(buffer, offset, nCount);
            await this.socket.SendAsync(sendbuffer, WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        /// <summary>
        /// 关闭当前连接
        /// </summary>
        /// <returns></returns>
        public async Task Close()
        {
            await Close(WebSocketCloseStatus.Empty, "");
        }

        /// <summary>
        /// 关闭当前连接 病返回一个状态
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="description">消息</param>
        /// <returns></returns>
        public async Task Close(WebSocketCloseStatus status, string description)
        {
            if (this.socket == null)
            {
                throw new Exception("socket is null");
            }
            if (this.socket.State == WebSocketState.Open)
            {
                await this.socket.CloseAsync(status, description, CancellationToken.None);
            }
        }
        #endregion









        #region Property

        private ConcurrentDictionary<String, Object> valuePairs { get; set; }

        public Encoding Encoding
        {
            get
            {
                return controller.Encoding;
            }
        }

        private WebSocketController controller;

        public bool IsConnect
        {
            get
            {
                return socket.State == WebSocketState.Open;
            }
        }

        #endregion
    }
}
