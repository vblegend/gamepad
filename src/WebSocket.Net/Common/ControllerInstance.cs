using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WebSocketServer.Net.Common
{
    /// <summary>
    /// websocket控制器实例
    /// </summary>
    internal class ControllerInstance
    {
        internal ControllerInstance(String path, WebSocketController controller)
        {
            Controller = controller;
            Path = path;
            Paths = path.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            Regex = new Regex(path);
        }

        /// <summary>
        /// 完整路径
        /// </summary>
        internal String Path { get; private set; }

        /// <summary>
        /// 路径数组
        /// </summary>
        internal String[] Paths { get; private set; }


        /// <summary>
        /// 路径数组
        /// </summary>
        internal Regex Regex { get; private set; }




        /// <summary>
        /// WebSocket 控制器
        /// </summary>
        internal WebSocketController Controller { get; private set; }
    }
}
