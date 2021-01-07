using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketServer.Net
{
    /// <summary>
    /// WebSocket 控制器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class WebSocketsAttribute : Attribute
    {
        /// <summary>
        /// 映射路径 
        /// </summary>
        public String Path { get; private set; }
        /// <summary>
        /// 指明该类为 WebSocket 控制器
        /// </summary>
        /// <param name="path">映射路径的正则表达式</param>
        public WebSocketsAttribute(string mapPath)
        {
            Path = mapPath;
        }
    }
}
