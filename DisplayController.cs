using System;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using Grommet.Ext;
using NetMf.CommonExtensions;

namespace BikeComputer
{
    public class DisplayController
    {

        enum LinesEnum
        {
            Line1,
            Line2
        }

        SpeedTimeDisplayProcessor speedTimeCtrl = null;
        OdometerTripTimeDisplayProcessor odoTripCtrl = null;
        WheelSizeDisplayProcessor wheelCtrl = null;
        SetTimeDisplayProcessor setTimeCtrl = null;

        BikeData bikeData = null;

        IDisplayProcessor currentLine1 = null;
        IDisplayProcessor currentLine2 = null;

        public DisplayController(BikeData inBikeData)
        {
            bikeData = inBikeData;

            // Setup views
            speedTimeCtrl = new SpeedTimeDisplayProcessor(bikeData);
            odoTripCtrl = new OdometerTripTimeDisplayProcessor(bikeData);
            wheelCtrl = new WheelSizeDisplayProcessor(bikeData);
            setTimeCtrl = new SetTimeDisplayProcessor(bikeData);

            Initialise();
        }

        private void Initialise()
        {
            currentLine1 = speedTimeCtrl;
            currentLine2 = odoTripCtrl;
        }

        public void ProcessKey(FEZ_Shields.KeypadLCD.Keys keyPressed)
        {

            if (currentLine1.IsProcessingKey())
            {
                currentLine1.ProcessKey(keyPressed);
            }
            else if (currentLine2.IsProcessingKey())
            {
                currentLine2.ProcessKey(keyPressed);
            }
            else
            {
                switch (keyPressed)
                {
                    case FEZ_Shields.KeypadLCD.Keys.Up:
                        ClearLine(LinesEnum.Line2);

                        if (currentLine2 is WheelSizeDisplayProcessor)
                        {
                            currentLine2 = odoTripCtrl;
                        }
                        else if (currentLine2 is SetTimeDisplayProcessor)
                        {
                            currentLine2 = wheelCtrl;
                        }

                        break;

                    case FEZ_Shields.KeypadLCD.Keys.Down:
                        ClearLine(LinesEnum.Line2);

                        if (currentLine2 is OdometerTripTimeDisplayProcessor)
                        {
                            currentLine2 = wheelCtrl;
                        }
                        else if (currentLine2 is WheelSizeDisplayProcessor)
                        {
                            currentLine2 = setTimeCtrl;
                        }

                        break;

                    case FEZ_Shields.KeypadLCD.Keys.Select:
                        currentLine2.BeginEdit();
                        break;
                }
            }
        }
        
        public void DisplayLines()
        {
            if ((currentLine1 == null) && (currentLine2 == null))
            {
                FEZ_Shields.KeypadLCD.Clear();
            }
            else if ((currentLine1 == null) && (currentLine2 != null))
            {
                currentLine1.Display();
                ClearLine(LinesEnum.Line2);
            }
            else if ((currentLine1 != null) && (currentLine2 == null))
            {
                ClearLine(LinesEnum.Line1);
                currentLine2.Display();
            }
            else
            {
                currentLine1.Display();
                currentLine2.Display();
            }
        }

        private void ClearLine(LinesEnum en)
        {

            string EmptyLine = new string(' ', 16);
            
            switch (en)
            {
                case LinesEnum.Line1:
                    FEZ_Shields.KeypadLCD.SetCursor(0, 0);
                    FEZ_Shields.KeypadLCD.Print(EmptyLine);
                    break;
                case LinesEnum.Line2:
                    FEZ_Shields.KeypadLCD.SetCursor(1, 0);
                    FEZ_Shields.KeypadLCD.Print(EmptyLine);
                    break;
            }
        }

        public bool CanEditLine()
        {
            return currentLine1.CanEditLine() || currentLine2.CanEditLine();
        }
    }    
}
