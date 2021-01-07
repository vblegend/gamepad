using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebSocketServer.Net.Common;

namespace WebSocketServer.Net
{
    /// <summary>
    /// websocket 控制器管理器
    /// </summary>
    internal static class WebSocketControllerManager
    {
        internal static IServiceProvider ServiceProvider { get; private set; }
        /// <summary>
        /// 初始化环境
        /// </summary>
        /// <param name="receiveBufferSize"></param>
        internal static void InitContext(IApplicationBuilder app)
        {
            WebSocketControllerManager.ServiceProvider = app.ApplicationServices;
            Controllers = new List<ControllerInstance>();
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            Type sessiontype = typeof(WebSocketController);
            Type websocketAttributetype = typeof(WebSocketsAttribute);

            foreach (var assembly in assemblys)
            {
                Type[] types = assembly.GetTypes();

                foreach (var classly in types)
                {
                    if (classly.IsClass && !classly.IsAbstract && classly.IsPublic && classly.IsSubclassOf(sessiontype))
                    {
                        var attribure = classly.GetCustomAttribute(websocketAttributetype) as WebSocketsAttribute;
                        if (attribure != null)
                        {
                            WebSocketController controller = null;
                            List<Object> paramer = new List<Object>();
                            var constructors = classly.GetConstructors();
                            foreach (var constructor in constructors)
                            {
                                foreach (var item in constructor.GetParameters())
                                {
                                    paramer.Add(WebSocketControllerManager.ServiceProvider.GetService(item.ParameterType));
                                }
                                if (constructor.GetParameters().Length == paramer.Count)
                                {
                                    controller = Activator.CreateInstance(classly, paramer.ToArray()) as WebSocketController;
                                    break;
                                }
                            }
                            if (controller == null)
                            {
                                controller = Activator.CreateInstance(classly) as WebSocketController;
                            }
                            Autowired(controller);
                            var instance = new ControllerInstance(attribure.Path, controller);
                            Controllers.Add(instance);
                        }
                    }
                }
            }
        }











        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        internal static async Task PipelineHandle(HttpContext context, Func<Task> next)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var controller = WebSocketControllerManager.MatchingController(context.Request.Path);
                if (controller != null)
                {
                    await controller.AcceptWebSocketAsync(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await next();
            }
        }



        public static void Autowired(object service)
        {
            var serviceType = service.GetType();
            //字段赋值
            foreach (FieldInfo field in serviceType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var autowiredAttr = field.GetCustomAttribute<AutowiredAttribute>();
                if (autowiredAttr != null)
                {
                    field.SetValue(service, WebSocketControllerManager.ServiceProvider.GetService(field.FieldType));
                }
            }
            //属性赋值
            foreach (PropertyInfo property in serviceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var autowiredAttr = property.GetCustomAttribute<AutowiredAttribute>();
                if (autowiredAttr != null)
                {
                    property.SetValue(service, WebSocketControllerManager.ServiceProvider.GetService(property.PropertyType));
                }
            }
        }















        /// <summary>
        /// 匹配控制器
        /// </summary>
        /// <param name="mapPath"></param>
        /// <returns></returns>
        internal static WebSocketController MatchingController(String mapPath)
        {
            foreach (var instance in Controllers)
            {
                if (instance.Regex.IsMatch(mapPath))
                {
                    return instance.Controller;
                }
            }
            return null;
            //var paths = mapPath.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (var instance in Controllers)
            //{
            //    if (matchingArray(paths, instance.Paths))
            //    {
            //        return instance.Controller;
            //    }
            //}
            //return null;
        }

        private static Boolean matchingArray(String [] source, String [] dest)
        {
            if (source.Length == dest.Length)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    if (source[i] != dest[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }





        #region Property


        private static List<ControllerInstance> Controllers { get; set; }



        private static ConcurrentDictionary<Type, Object> InjectionParameters { get; set; }






        #endregion
    }
}
