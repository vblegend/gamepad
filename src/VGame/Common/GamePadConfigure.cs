using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gamepad.Common
{


    public enum GamePadGPIOMode
    { 
        /// <summary>
        /// 自动检测端口
        /// </summary>
        Auto,
        /// <summary>
        /// 指定端口
        /// </summary>
        Fixed
    }





    public class GamePadConfigure
    {
        /// <summary>
        /// gpio模式
        /// </summary>
        public GamePadGPIOMode Mode { get; set; }

        /// <summary>
        /// 手柄通用调度引脚
        /// </summary>
        public Int32 LatchPin { get; set; }

        /// <summary>
        /// 手柄通用时钟引脚
        /// </summary>
        public Int32 ClkPin { get; set; }

        /// <summary>
        /// 手柄1 数据引脚
        /// </summary>
        public Int32 Data1Pin { get; set; }


        /// <summary>
        /// 手柄2 数据引脚
        /// </summary>
        public Int32 Data2Pin { get; set; }




        /// <summary>
        /// 控制器读取手柄延迟时间 微秒
        /// </summary>
        public UInt32 DelayTime { get; set; }

        /// <summary>
        /// 重复按下事件 大于零是为重复间隔 纳秒
        /// </summary>
        public UInt32 keyRepeatPressEvent { get; set; }



     //   ContinuePress



    }
}
