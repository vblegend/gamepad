using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using WebSocketServer.Net.Common;

namespace WebSocketServer.Net
{
    /// <summary>
    /// .net Core WebSocket 服务集成
    /// </summary>
    public static class WebSocketExtend
    {

        /// <summary>
        /// 启用 WebSocket 服务器
        /// </summary>
        /// <param name="app"></param>
        /// <param name="receiveBufferSize"></param>
        /// <returns></returns>
        public static IApplicationBuilder EnabledWebSocket(this IApplicationBuilder app)
        {
            return EnabledWebSocket(app, null);
        }


        /// <summary>
        /// 启用 WebSocket 服务器 并支持对象注入
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options">WebSocket选项</param>
        /// <param name="injectionDic">注入对象字典</param>
        /// <returns></returns>
        public static IApplicationBuilder EnabledWebSocket(this IApplicationBuilder app, WebSocketOptions options)
        {
            WebSocketControllerManager.InitContext(app);
            if (options != null)
            {
                app.UseWebSockets(options);
            }
            else
            {
                app.UseWebSockets();
            }
            app.Use(WebSocketControllerManager.PipelineHandle);
            return app;
        }

    }
}
