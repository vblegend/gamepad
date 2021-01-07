using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using gamepad.Common;
using gamepad.Providers;
using WebSocketServer.Net;

namespace gamepad.WebSocketControllers
{
    [WebSockets("/gamepad/*")]
    public class GamepadController : WebSocketController<GamepadController>
    {
        public GamepadController(ILoggerFactory loggerfactory, GamePadGpio gamepad, GamePadConfigureProvider configureProvider) : base(loggerfactory)
        {
            configureProvider.Load();
            gamepad.Start(configureProvider);
            gamepad.onKeyChange += Padreader_onKeyChange;
            logger.LogDebug("WebSocket 服务启动。。。");
        }

        private async void Padreader_onKeyChange(GamepadKey key)
        {
            System.Console.WriteLine(JsonConvert.SerializeObject(key));
            await this.SendAll(JsonConvert.SerializeObject(key));

        }
    }
}
