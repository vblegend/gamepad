using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gamepad.Common
{

    /// <summary>
    /// 游戏手柄按键
    /// </summary>
    public class GamepadKey
    {
        public GamepadKey(GAMEPADS gAMEPAD_1, GAMEPADKEYS i)
        {
            this.pad = gAMEPAD_1;
            this.key = i;
        }

        /// <summary>
        /// 按键ID {手柄-按键}
        /// </summary>
        public String keyId
        {
            get
            {
                return $"{pad}-{key}";
            }
        }
        /// <summary>
        /// 触发事件的手柄
        /// </summary>
        public GAMEPADS pad { get; set; }

        /// <summary>
        /// 触发事件的按键
        /// </summary>
        public GAMEPADKEYS key { get; set; }

        /// <summary>
        /// 按键的状态
        /// </summary>
        public KEY_STATE state { get; set; }

        /// <summary>
        /// 什么时候按下的或什么时候弹起的
        /// </summary>
        public UInt32 timestamp { get; set; }

        /// <summary>
        /// 按键按下后触发了多少次
        /// </summary>
        public UInt32? count { get; set; }

        /// <summary>
        /// 按下持续的时间
        /// </summary>
        public UInt32? duration { get; set; }

        /// <summary>
        /// 上次更新时间
        /// </summary>
        public UInt32 lastupdate { get; set; }

    }



    public delegate void KeyHandleEvent();


}
