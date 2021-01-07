using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebSocketServer.Net.Common;

namespace WebSocketServer.Net
{
    /// <summary>
    /// WebSocket 上下文对象
    /// </summary>
    public interface ISocketContext
    {
        /// <summary>
        /// 当前连接对象
        /// </summary>
        WebSocket socket { get; }

        /// <summary>
        /// 获取字典数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(String key);

        /// <summary>
        /// 设置字典数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Set<T>(String key, T value);

        /// <summary>
        /// 当前连接是否正常
        /// </summary>
        Boolean IsConnect { get; }

        /// <summary>
        /// 编码/解码字符集
        /// </summary>
        Encoding Encoding { get; }

        /// <summary>
        /// 发送字符串文本
        /// </summary>
        /// <param name="text">字符串内容</param>
        /// <returns></returns>
        Task Send(String text);


        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <returns></returns>
        Task Send(Byte[] buffer);

        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="offset">数据的开始位子</param>
        /// <param name="nCount">数据的长度</param>
        /// <returns></returns>
        Task Send(Byte[] buffer, Int32 offset, Int32 nCount);

        /// <summary>
        /// 关闭当前连接
        /// </summary>
        /// <returns></returns>
        Task Close();

        /// <summary>
        /// 关闭当前连接 病给出一个状态
        /// </summary>
        /// <param name="status">关闭状态</param>
        /// <param name="description">关闭消息</param>
        /// <returns></returns>
        Task Close(WebSocketCloseStatus status, String description);

    }
}
