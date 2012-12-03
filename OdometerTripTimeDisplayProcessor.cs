using System;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using Grommet.Ext;
using NetMf.CommonExtensions;

namespace BikeComputer
{
    class OdometerTripTimeDisplayProcessor : IDisplayProcessor
    {

        private BikeData bikeData = null;

        public OdometerTripTimeDisplayProcessor(BikeData inBikeData)
        {
            bikeData = inBikeData;
        }

        public void Display()
        {

            String odometerText = StringUtility.Format("{0:F3} km", bikeData.OdoDone);

            String timerText = String.Empty;
            if (bikeData.IsStartTripSet)
            {
                TimeSpan tripSpan = DateTime.Now - bikeData.StartTrip;
                timerText = StringUtility.Format("{0:T}", tripSpan);
            }
            else
            {
                timerText = "00:00:00";
            }

            // Odometer
            FEZ_Shields.KeypadLCD.SetCursor(1, 0);
            FEZ_Shields.KeypadLCD.Print(odometerText);

            // Timer                
            FEZ_Shields.KeypadLCD.SetCursor(1, 11);
            FEZ_Shields.KeypadLCD.Print(timerText.Substring(0, 5));

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
