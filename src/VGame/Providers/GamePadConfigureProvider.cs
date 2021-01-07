using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gamepad.Common;

namespace gamepad.Providers
{
    public class GamePadConfigureProvider
    {
        public GamePadConfigure configure { get; private set; }

        private const String ConfigureFileName = "setting.json";


        public void Load()
        {
            if (this.configure == null)
            {
                var jsonString = File.ReadAllText(GamePadConfigureProvider.ConfigureFileName, Encoding.UTF8);
                this.configure = JsonConvert.DeserializeObject<GamePadConfigure>(jsonString);
            }
        }




        public void Save()
        {
            var jsonString = JsonConvert.SerializeObject(this.configure);
            File.WriteAllText(GamePadConfigureProvider.ConfigureFileName, jsonString, Encoding.UTF8);
        }







    }
}
