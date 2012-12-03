using System;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using Grommet.Ext;
using NetMf.CommonExtensions;

namespace BikeComputer
{
    class SpeedTimeDisplayProcessor : IDisplayProcessor
    {

        private BikeData bikeData = null;

        public SpeedTimeDisplayProcessor(BikeData inBikeData)
        {
            bikeData = inBikeData;
        }

        public void Display()
        {

            String speedText = StringUtility.Format("{0:G} km/h", (int)bikeData.CurrentSpeed);
            String timeText = StringUtility.Format("{0:T}", DateTime.Now);

            // Speed
            if (speedText.Length == 6)
            {
                // Clear the previous speed
                FEZ_Shields.KeypadLCD.SetCursor(0, 0);
                FEZ_Shields.KeypadLCD.Print(" ");
                FEZ_Shields.KeypadLCD.SetCursor(0, 1);
                FEZ_Shields.KeypadLCD.Print(speedText);
                FEZ_Shields.KeypadLCD.SetCursor(0, 7);
                FEZ_Shields.KeypadLCD.Print(" ");
            }
            else if (speedText.Length == 7)
            {
                FEZ_Shields.KeypadLCD.SetCursor(0, 0);
                FEZ_Shields.KeypadLCD.Print(speedText);
                FEZ_Shields.KeypadLCD.SetCursor(0, 7);
                FEZ_Shields.KeypadLCD.Print(" ");
            }
            else
            {
                FEZ_Shields.KeypadLCD.SetCursor(0, 0);
                FEZ_Shields.KeypadLCD.Print(speedText);
            }

            // Time
            FEZ_Shields.KeypadLCD.SetCursor(0, 8);
            FEZ_Shields.KeypadLCD.Print(timeText);

        }

        public bool IsProcessingKey()
        {
            return false;
        }

        public void ProcessKey(FEZ_Shields.KeypadLCD.Keys keyPressed)
        {
            return;
        }

        public void BeginEdit()
        {
            return;
        }
        
        public bool CanEditLine()
        {
            return false;
        }
    }
}
