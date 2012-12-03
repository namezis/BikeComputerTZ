using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace GHIElectronics.NETMF.FEZ
{
    public static partial class FEZ_Shields
    {
        static public class KeypadLCD
        {
            public enum Keys
            {
                Up,
                Down,
                Right,
                Left,
                Select,
                None,
            }

            static OutputPort LCD_RS;
            static OutputPort LCD_E;

            static OutputPort LCD_D4;
            static OutputPort LCD_D5;
            static OutputPort LCD_D6;
            static OutputPort LCD_D7;

            static AnalogIn AnKey;
            

            static OutputPort BackLight;

            const byte DISPLAY_ON_NO_CURSOR = 0x0C;
            const byte DISPLAY_ON_SOLID_CURSOR = 0x0E;
            const byte DISPLAY_ON_BLINKING_CURSOR = 0x0F;

            const byte DISPLAY_OFF = 0x08;

            const byte MOVE_CURSOR_LEFT_FIRST_LINE = 0x80;
            const byte MOVE_CURSOR_LEFT_SECOND_LINE = 0xC0;
            const byte MOVE_CURSOR_LEFT_THIRD_LINE = 0x94;
            const byte MOVE_CURSOR_LEFT_FOURTH_LINE = 0xD4;

            const byte CLR_DISP = 1;      //Clear display
            const byte CUR_HOME = 2;      //Move cursor home and clear screen memory
            
            const byte MAKE_CURSOR_BLINK_BLOCK = 0x0F;
            const byte MAKE_CURSOR_INVISIBLE = 0x0C;

            public static void Initialize()
            {
                LCD_RS = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di8, false);
                LCD_E = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di9, false);

                LCD_D4 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di4, false);
                LCD_D5 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di5, false);
                LCD_D6 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di6, false);
                LCD_D7 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di7, false);

                AnKey = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An0);

                BackLight = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di10, true);

                LCD_RS.Write(false);

                // 4 bit data communication
                Thread.Sleep(50);

                LCD_D7.Write(false);
                LCD_D6.Write(false);
                LCD_D5.Write(true);
                LCD_D4.Write(true);

                LCD_E.Write(true);
                LCD_E.Write(false);

                Thread.Sleep(50);
                LCD_D7.Write(false);
                LCD_D6.Write(false);
                LCD_D5.Write(true);
                LCD_D4.Write(true);

                LCD_E.Write(true);
                LCD_E.Write(false);

                Thread.Sleep(50);
                LCD_D7.Write(false);
                LCD_D6.Write(false);
                LCD_D5.Write(true);
                LCD_D4.Write(true);

                LCD_E.Write(true);
                LCD_E.Write(false);

                Thread.Sleep(50);
                LCD_D7.Write(false);
                LCD_D6.Write(false);
                LCD_D5.Write(true);
                LCD_D4.Write(false);

                LCD_E.Write(true);
                LCD_E.Write(false);

                SendCmd(DISPLAY_ON_NO_CURSOR);
                SendCmd(CLR_DISP);
            }

            //Sends an ASCII character to the LCD
            static void Putc(byte c)
            {
                LCD_D7.Write((c & 0x80) != 0);
                LCD_D6.Write((c & 0x40) != 0);
                LCD_D5.Write((c & 0x20) != 0);
                LCD_D4.Write((c & 0x10) != 0);
                LCD_E.Write(true); LCD_E.Write(false); //Toggle the Enable Pin

                LCD_D7.Write((c & 0x08) != 0);
                LCD_D6.Write((c & 0x04) != 0);
                LCD_D5.Write((c & 0x02) != 0);
                LCD_D4.Write((c & 0x01) != 0);
                LCD_E.Write(true); LCD_E.Write(false); //Toggle the Enable Pin
                //Thread.Sleep(1);
            }

            //Sends an LCD command
            static void SendCmd(byte c)
            {
                LCD_RS.Write(false); //set LCD to data mode

                LCD_D7.Write((c & 0x80) != 0);
                LCD_D6.Write((c & 0x40) != 0);
                LCD_D5.Write((c & 0x20) != 0);
                LCD_D4.Write((c & 0x10) != 0);
                LCD_E.Write(true); LCD_E.Write(false); //Toggle the Enable Pin

                LCD_D7.Write((c & 0x08) != 0);
                LCD_D6.Write((c & 0x04) != 0);
                LCD_D5.Write((c & 0x02) != 0);
                LCD_D4.Write((c & 0x01) != 0);
                LCD_E.Write(true); LCD_E.Write(false); //Toggle the Enable Pin
                Thread.Sleep(1);
                LCD_RS.Write(true); //set LCD to data mode
            }

            public static void Print(string str)
            {
                for (int i = 0; i < str.Length; i++)
                    Putc((byte)str[i]);
            }

            public static void Clear()
            {
                SendCmd(CLR_DISP);
            }

            public static void CursorHome()
            {
                SendCmd(CUR_HOME);
            }
            
            public static void SetCursor(byte row, byte col)
            {
                SendCmd((byte)(MOVE_CURSOR_LEFT_FIRST_LINE | row << 6 | col));
            }

            public static Keys GetKey()
            {
                int i = AnKey.Read();
          
                if (i < 1050 && i >= 1000)
                    return Keys.None;

                if (i < 50 && i >= 0)
                    return Keys.Right;

                if (i < 250 && i >= 180)
                    return Keys.Up;

                if (i < 550 && i >= 400)
                    return Keys.Down;

                if (i < 790 && i >= 650)
                    return Keys.Left;

                if (i < 999 && i >= 830)
                    return Keys.Select;

                return Keys.None;
            }
            
            public static void TurnBacklightOn()
            {
                BackLight.Write(true);
            }
            
            public static void ShutBacklightOff()
            {
                BackLight.Write(false);
            }

            public static void TurnDisplayOn()
            {
                SendCmd(DISPLAY_ON_NO_CURSOR);
            }

            public static void ShutDisplayOff()
            {
                SendCmd(DISPLAY_OFF);
            }

            public static void ShowCursor()
            {
                SendCmd(MAKE_CURSOR_BLINK_BLOCK);
            }

            public static void HideCursor()
            {
                SendCmd(MAKE_CURSOR_INVISIBLE);
            }
        }
    }
}