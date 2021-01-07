using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gamepad.Common;

namespace gamepad.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettingController : ControllerBase
    {


        private readonly ILogger<WeatherForecastController> _logger;

        public SettingController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }



        [HttpGet]
        public GamePadConfigure Get()
        {
            return new GamePadConfigure();
        }



        [HttpPut]
        public void Put(GamePadConfigure configure)
        {
 
        }





    }
}
