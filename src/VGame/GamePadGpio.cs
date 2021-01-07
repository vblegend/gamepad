using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using gamepad.Common;
using gamepad.Providers;

namespace gamepad
{

    public delegate void GamepadKeyEventHandle(GamepadKey key);


    public class GamePadGpio
    {
        private GamepadKey[][] gamepads { get; set; }
        private GpioController Latch { get; set; }
        public Int32 latchPin { get; set; }
        private GpioController Clk { get; set; }
        public Int32 clkPin { get; set; }
        private GpioController Data1 { get; set; }
        public Int32 data1Pin { get; set; }
        private GpioController Data2 { get; set; }
        public Int32 data2Pin { get; set; }
        private Thread hThread { get; set; }
        public UInt32 delayTime { get; set; }
        public UInt32 keyRepeatPressEvent { get; set; }
        public event GamepadKeyEventHandle onKeyChange;


        private GpioController Direction { get; set; }
        private Int32 directionPin { get; set; }


        // clk 14  data1 15 data2 18  latch 20             gpio 26 = 1
        // clk 13  data1 6  data2 5   latch 2              gpio 26 = 1





        public GamePadGpio()
        {
            this.directionPin = 26;
            this.latchPin = 16;
            this.clkPin = 12;
            this.data1Pin = 20;
            this.data2Pin = 21;
            this.delayTime = 1000;
            this.keyRepeatPressEvent = 0;
            this.gamepads = new GamepadKey[2][];
            this.gamepads[0] = new GamepadKey[8];
            this.gamepads[1] = new GamepadKey[8];
            this.Direction = new GpioController(PinNumberingScheme.Logical);






            for (int i = 0; i < 8; i++)
            {
                this.gamepads[0][i] = new GamepadKey(GAMEPADS.GAMEPAD_1, (GAMEPADKEYS)i);
                this.gamepads[1][i] = new GamepadKey(GAMEPADS.GAMEPAD_2, (GAMEPADKEYS)i);
            }
        }


        public void Start(GamePadConfigureProvider configureProvider)
        {
            if (this.hThread == null)
            {

                this.Direction.OpenPin(this.directionPin, PinMode.InputPullUp);

                // clk 14  data1 15 data2 18  latch 20             gpio 26 = 1
                // clk 13  data1 6  data2 5   latch 2              gpio 26 = 1


                if (this.Direction.Read(this.directionPin) == PinValue.High)
                {
                    this.clkPin = 14;
                    this.data1Pin = 15;
                    this.data2Pin = 18;
                    this.latchPin = 20;
                    Console.WriteLine("向外");
                }
                else
                {
                    this.clkPin = 13;
                    this.data1Pin = 6;
                    this.data2Pin = 5;
                    this.latchPin = 2;
                    Console.WriteLine("向内");
                }
                //this.latchPin = configureProvider.configure.LatchPin;
                //this.clkPin = configureProvider.configure.ClkPin;
                //this.data1Pin = configureProvider.configure.Data1Pin;
                //this.data2Pin = configureProvider.configure.Data2Pin;
                this.delayTime = configureProvider.configure.DelayTime;
                this.keyRepeatPressEvent = configureProvider.configure.keyRepeatPressEvent;



                this.hThread = new Thread(this.Key_Proc)
                {
                    IsBackground = true
                };
                this.hThread.Start();
            }
        }


        public void Stop()
        {
            if (this.hThread != null)
            {
                this.Direction.ClosePin(this.directionPin);
                this.hThread.Interrupt();
                this.hThread = null;
            }

        }

        private void Key_Proc()
        {
            try
            {
                Wiringpi.WiringPiSetup();
                Latch = new GpioController(PinNumberingScheme.Logical);
                Latch.OpenPin(latchPin, PinMode.Output);

                Clk = new GpioController(PinNumberingScheme.Logical);
                Clk.OpenPin(clkPin, PinMode.Output);

                Data1 = new GpioController(PinNumberingScheme.Logical);
                Data1.OpenPin(data1Pin, PinMode.InputPullUp);

                Data2 = new GpioController(PinNumberingScheme.Logical);
                Data2.OpenPin(data2Pin, PinMode.InputPullUp);

                while (true)
                {
                    UInt32 tick = Wiringpi.Micros();
                    Int32 index = 0;
                    Latch.Write(latchPin, PinValue.High);
                    Wiringpi.delayMicroseconds(1);
                    Latch.Write(latchPin, PinValue.Low);
                    Wiringpi.delayMicroseconds(1);
                    while (index < 8)
                    {
                        this.updateKeyState(this.gamepads[0][index], Data1.Read(data1Pin), tick);
                        this.updateKeyState(this.gamepads[1][index], Data2.Read(data2Pin), tick);
                        Wiringpi.delayMicroseconds(1);
                        Clk.Write(clkPin, PinValue.High);
                        Wiringpi.delayMicroseconds(1);
                        Clk.Write(clkPin, PinValue.Low);
                        Wiringpi.delayMicroseconds(1);
                        index++;
                    }
                    Wiringpi.delayMicroseconds(5000);
                }

            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Latch.ClosePin(latchPin);
                Clk.ClosePin(clkPin);
                Data1.ClosePin(data1Pin);
                Data2.ClosePin(data2Pin);
                this.hThread = null;
            }
        }



        /// <summary>
        /// 更新按键状态
        /// </summary>
        /// <param name="key">按键</param>
        /// <param name="state">状态</param>
        /// <param name="tick">时间戳</param>
        public void updateKeyState(GamepadKey key, PinValue state, UInt32 tick)
        {
            var value = state == PinValue.Low ? KEY_STATE.PRESSED : KEY_STATE.NONE;
            if (key.state == value)
            {
                if (value == KEY_STATE.NONE) return;
                if (this.keyRepeatPressEvent == 0 || tick - key.lastupdate < this.keyRepeatPressEvent) return;

                key.count++;
                key.duration = tick - key.timestamp;
                key.lastupdate = tick;
            }
            else
            {
                if (value == KEY_STATE.PRESSED)
                {
                    // 按下瞬间
                    if (tick - key.lastupdate < 50000)
                    {
                        return;
                    }
                }
                else
                {
                    // 抬起瞬间 从按下到抬起 时间太短的话 过滤掉
                    if (tick - key.timestamp < 100000)
                    {
                        return;
                    }

                }
                key.count = 1;
                key.state = value;
                key.timestamp = tick;
                key.duration = null;
                key.lastupdate = tick;
            }
            // 触发事件
            this.onKeyChange?.Invoke(key);
        }
    }
}
