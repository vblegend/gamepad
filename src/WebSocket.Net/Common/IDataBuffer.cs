using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace WebSocketServer.Net.Common
{
    /// <summary>
    /// 接收数据缓冲区
    /// </summary>
    public interface IDataBuffer : IDisposable
    {
        /// <summary>
        /// 收到的消息数据
        /// </summary>
        WebSocketMessageType Type { get; }

        /// <summary>
        /// 二进制数据
        /// </summary>
        Byte[] Binary { get; }

        /// <summary>
        /// 文本数据
        /// </summary>
        String Text { get; }

        /// <summary>
        /// 数据长度
        /// </summary>
        Int32 Count { get; }

        /// <summary>
        /// 解码字符集
        /// </summary>
        Encoding Encoding { get;}
    }
}
