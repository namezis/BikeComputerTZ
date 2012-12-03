using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using Grommet.Ext;
using NetMf.CommonExtensions;
using BikeComputer;

namespace FEZ_Panda_Application1
{
    public class Program
    {
        
        private static BikeData bikeData = new BikeData();

        public static void Main()
        {

            ////////////////////////////////////////////////////////
            // BEGIN  Computer variables
            ////////////////////////////////////////////////////////

            Utility.SetLocalTime(new DateTime(2011, 01, 01, 12, 0, 0, 0));

            // Blink board LED
            bool blinkLEDState = false;

            // test blink led
            OutputPort blinkLED = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, blinkLEDState);

            // Sensor port
            InterruptPort IntSensorButton = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.Di1, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            // add an interrupt handler to the pin
            IntSensorButton.OnInterrupt += new NativeEventHandler(IntSensorButton_OnInterrupt);

            // Switch displey on/off port
            InterruptPort IntDisplayOnOffButton = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.Di2, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            // add an interrupt handler to the pin
            IntDisplayOnOffButton.OnInterrupt += new NativeEventHandler(IntDisplayOnOffButton_OnInterrupt);

            FEZ_Shields.KeypadLCD.Initialize();            
            bikeData.IsDisplayOn = true;

            DisplayController display = new DisplayController(bikeData);

            /////////////////////////////////////////////////////////
            // END  Computer variables
            /////////////////////////////////////////////////////////
                        
            // Main loop
            while (true)
            {
                //                          |0123456789012345
                //                         0|99 km/h 99:99:99
                //                         1|99.999 km  99:99
                //             Edit 1        Wheel size: 999>
                //             Edit 2        Time: 99:99:99 >

                // Reset speedometer
                if (display.CanEditLine())
                {
                    bikeData.lastDisplayUsed = DateTime.Now;
                }
                else
                {
                    TimeSpan lastDisplayUsedSpan = DateTime.Now - bikeData.lastDisplayUsed;
                    if (lastDisplayUsedSpan.Seconds > 5)
                    {
                        bikeData.lastSpeedCheck = DateTime.MinValue;
                        bikeData.CurrentSpeed = 0;
                        bikeData.IsDisplayOn = false;
                    }
                }                
                
                // Reset trip timer
                TimeSpan lastTripCheckSpan = DateTime.Now - bikeData.lastTripCheck;
                if (lastTripCheckSpan.Minutes > 30)
                {
                    bikeData.IsStartTripSet = false;
                    bikeData.StartTrip = DateTime.MinValue;
                }

                // Sleep for 500 milliseconds
                Thread.Sleep(500);
                                
                display.DisplayLines();
                
                FEZ_Shields.KeypadLCD.Keys KeyPressed = FEZ_Shields.KeypadLCD.GetKey();
                if (KeyPressed != FEZ_Shields.KeypadLCD.Keys.None)
                {
                    if (!bikeData.IsDisplayOn)
                    {
                        bikeData.IsDisplayOn = true;
                    }
                    else
                    {
                        display.ProcessKey(KeyPressed);
                    }
                }
                                
                // toggle LED state
                blinkLEDState = !blinkLEDState;
                blinkLED.Write(blinkLEDState);
            }
        }

        /// <summary>
        /// Speed Sensor Interrupt 
        /// </summary>
        static void IntSensorButton_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (!bikeData.IsDisplayOn)
            {                
                bikeData.IsDisplayOn = true;
            }

            DateTime newSpeedCheck = DateTime.Now;
            bikeData.lastTripCheck = DateTime.Now;

            double diffInMs = (newSpeedCheck.Ticks - bikeData.lastSpeedCheck.Ticks) / TimeSpan.TicksPerMillisecond;

            // Do not measure speed for next 40 ms
            if ((bikeData.lastSpeedCheck != DateTime.MinValue) && (diffInMs > 40))
            {
                bikeData.CurrentSpeed = (bikeData.WheelSize / diffInMs) * 36;
                bikeData.OdoDone += ((double)bikeData.WheelSize / 100000);

                if (!bikeData.IsStartTripSet)
                {
                    bikeData.IsStartTripSet = true;
                    bikeData.StartTrip = DateTime.Now;
                }
            }

            bikeData.lastSpeedCheck = newSpeedCheck;
            bikeData.lastDisplayUsed = newSpeedCheck;
        }

        /// <summary>
        /// Displey On/Off Interrupt 
        /// </summary>
        static void IntDisplayOnOffButton_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            DateTime nowDisplayOnCheck = DateTime.Now;
            double diffDisplayOnCheck = (nowDisplayOnCheck.Ticks - bikeData.lastDisplayOnCheck.Ticks) /TimeSpan.TicksPerMillisecond;
            if ((bikeData.lastDisplayOnCheck != DateTime.MinValue) && (diffDisplayOnCheck > 300))
            {
                bikeData.IsDisplayOn = !bikeData.IsDisplayOn;
            }
        }
    }
}
