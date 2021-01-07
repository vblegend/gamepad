using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace gamepad
{
    public class Wiringpi
    {

        private const String WiringPiLibrary = "libwiringPi.so";

        [DllImport(WiringPiLibrary, EntryPoint = "wiringPiSetup", SetLastError = true)]
        public static extern int WiringPiSetup();

        /// <summary>
        /// 延迟指定微秒树
        /// </summary>
        /// <param name="howLong"></param>

        [DllImport(WiringPiLibrary, EntryPoint = "delayMicroseconds")]
        public static extern void delayMicroseconds(UInt32 howLong);


        /// <summary>
        /// 获取一个调用wiringPiSetup函数以来到现在时间的毫秒数
        /// </summary>
        /// <returns>The result code.</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "millis", SetLastError = true)]
        public static extern uint Millis();

        /// <summary>
        /// 获取一个调用wiringPiSetup函数以来到现在时间的微秒数
        /// </summary>
        /// <returns>The result code.</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "micros", SetLastError = true)]
        public static extern UInt32 Micros();
    }
}
