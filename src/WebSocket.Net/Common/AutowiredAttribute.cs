using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketServer.Net.Common
{
    /// <summary>
    /// WebSocket 控制器属性自动注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AutowiredAttribute : Attribute
    {


    }
}
