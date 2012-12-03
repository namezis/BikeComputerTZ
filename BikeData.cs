using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.FEZ;

namespace BikeComputer
{
    public class BikeData
    {
        public double OdoDone = 0;
        public int WheelSize = 207;
        public double CurrentSpeed = 0;
        public bool IsStartTripSet = false;
        public DateTime StartTrip = DateTime.MinValue;
        public DateTime lastSpeedCheck = DateTime.MinValue;
        public DateTime lastTripCheck = DateTime.MinValue;
        private bool mIsDisplayOn = false;
        public DateTime lastDisplayOnCheck = DateTime.MinValue;
        public DateTime lastDisplayUsed = DateTime.MinValue;

        public bool IsDisplayOn
        {
            get
            {
                return mIsDisplayOn;
            }
            set
            {
                mIsDisplayOn = value;

                lastDisplayOnCheck = DateTime.Now;
                
                if (mIsDisplayOn)
                {
                    FEZ_Shields.KeypadLCD.TurnBacklightOn();
                    FEZ_Shields.KeypadLCD.TurnDisplayOn();
                    lastDisplayUsed = DateTime.Now;
                }
                else
                {
                    FEZ_Shields.KeypadLCD.ShutBacklightOff();
                    FEZ_Shields.KeypadLCD.ShutDisplayOff();
                }
            }
        }

    }
}
