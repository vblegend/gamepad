using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gamepad.Common
{
    /// <summary>
    /// 游戏手柄
    /// </summary>
    public enum GAMEPADS
    {
        /// <summary>
        /// 一号位手柄
        /// </summary>
        GAMEPAD_1 = 0,
        /// <summary>
        /// 二号位手柄
        /// </summary>
        GAMEPAD_2 = 1
    }

    /// <summary>
    /// 手柄按键
    /// </summary>
    public enum GAMEPADKEYS
    {
        /// <summary>
        /// B键
        /// </summary>
        KEY_B = 0,
        /// <summary>
        /// A键
        /// </summary>
        KEY_A = 1,
        /// <summary>
        /// 选择键
        /// </summary>
        KEY_SELETCT = 2,
        /// <summary>
        /// 开始键
        /// </summary>
        KEY_START = 3,
        /// <summary>
        /// 上按键
        /// </summary>
        KEY_UP = 4,
        /// <summary>
        /// 下
        /// </summary>
        KEY_DOWN = 5,
        /// <summary>
        /// 左
        /// </summary>
        KEY_LEFT = 6,
        /// <summary>
        /// 右
        /// </summary>
        KEY_RIGHT = 7
    }


    /// <summary>
    /// 按键状态
    /// </summary>
    public enum KEY_STATE
    {
        /// <summary>
        /// 未按下
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 按下的
        /// </summary>
        PRESSED = 1
    }
}
