using System;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using Grommet.Ext;
using NetMf.CommonExtensions;
using Microsoft.SPOT.Hardware;

namespace BikeComputer
{
    class SetTimeDisplayProcessor : IDisplayProcessor
    {

        enum EndEditEnum
        {
            Approving,
            Cancelling,
            NotEditing
        }

        const int HOUR_POSITION_0 = 0;
        const int HOUR_POSITION_1 = 1;
        const int MINUTE_POSITION_0 = 2;
        const int MINUTE_POSITION_1 = 3;
        const int SECOND_POSITION_0 = 4;
        const int SECOND_POSITION_1 = 5;
        const int EXIT_CHAR_POSITION = 6;

        private BikeData bikeData = null;
        private bool isInitialised = false;
        private bool isEditing = false;
        private char[] editingText = new char[6];
        private int editingPosition = HOUR_POSITION_0;
        private EndEditEnum approving = EndEditEnum.NotEditing;
        private bool blink = false;

        public SetTimeDisplayProcessor(BikeData inBikeData)
        {
            bikeData = inBikeData;
        }

        private void Initialise()
        {
            if (!isInitialised)
            {   
                String timeValue = StringUtility.Format("{0:T}", DateTime.Now);

                editingText[HOUR_POSITION_0] = timeValue[0];
                editingText[HOUR_POSITION_1] = timeValue[1];
                editingText[MINUTE_POSITION_0] = timeValue[3];
                editingText[MINUTE_POSITION_1] = timeValue[4];
                editingText[SECOND_POSITION_0] = timeValue[6];
                editingText[SECOND_POSITION_1] = timeValue[7];
                
                isInitialised = true;
            }
        }

        public void Display()
        {

            Initialise();

            FEZ_Shields.KeypadLCD.SetCursor(1, 0);
            FEZ_Shields.KeypadLCD.Print("Time:");

            char[] prepareText = new char[6];
            editingText.CopyTo(prepareText, 0);

            if (isEditing && blink && (editingPosition >= HOUR_POSITION_0 && editingPosition <= SECOND_POSITION_1))
            {
                prepareText[editingPosition] = ' ';
            }

            char[] displayText = new char[8];
            displayText[0] = prepareText[HOUR_POSITION_0];
            displayText[1] = prepareText[HOUR_POSITION_1];
            displayText[2] = ':';
            displayText[3] = prepareText[MINUTE_POSITION_0];
            displayText[4] = prepareText[MINUTE_POSITION_1];
            displayText[5] = ':';
            displayText[6] = prepareText[SECOND_POSITION_0];
            displayText[7] = prepareText[SECOND_POSITION_1];
            string strValue = new string(displayText);
            FEZ_Shields.KeypadLCD.SetCursor(1, 6);
            FEZ_Shields.KeypadLCD.Print(strValue);
            
            FEZ_Shields.KeypadLCD.SetCursor(1, 15);
            if (isEditing && blink && (editingPosition == EXIT_CHAR_POSITION))
            {
                FEZ_Shields.KeypadLCD.Print(" ");
            }
            else
            {
                switch (approving)
                {
                    case EndEditEnum.Approving:
                        FEZ_Shields.KeypadLCD.Print(">");
                        break;
                    case EndEditEnum.Cancelling:
                        FEZ_Shields.KeypadLCD.Print("X");
                        break;
                    case EndEditEnum.NotEditing:
                        FEZ_Shields.KeypadLCD.Print(" ");
                        break;
                }
            }

            blink = !blink;
        }

        public void ProcessKey(FEZ_Shields.KeypadLCD.Keys keyPressed)
        {
            switch (keyPressed)
            {
                case FEZ_Shields.KeypadLCD.Keys.Right:
                    editingPosition++;
                    if (editingPosition > EXIT_CHAR_POSITION)
                    {
                        editingPosition = EXIT_CHAR_POSITION;
                    }
                    break;

                case FEZ_Shields.KeypadLCD.Keys.Left:
                    editingPosition--;
                    if (editingPosition < HOUR_POSITION_0)
                    {
                        editingPosition = HOUR_POSITION_0;
                    }
                    break;

                case FEZ_Shields.KeypadLCD.Keys.Up:
                    if (editingPosition >= HOUR_POSITION_0 && editingPosition <= SECOND_POSITION_1)
                    {
                        char editedChar = editingText[editingPosition];
                        if (editedChar != '9')
                        {
                            editedChar++;
                            editingText[editingPosition] = editedChar;
                        }
                    }
                    else if (editingPosition == EXIT_CHAR_POSITION)
                    {
                        if (approving == EndEditEnum.Approving) approving = EndEditEnum.Cancelling;
                        else if (approving == EndEditEnum.Cancelling) approving = EndEditEnum.Approving;
                    }
                    break;

                case FEZ_Shields.KeypadLCD.Keys.Down:
                    if (editingPosition >= HOUR_POSITION_0 && editingPosition <= SECOND_POSITION_1)
                    {
                        char editedChar = editingText[editingPosition];
                        if (editedChar != '0')
                        {
                            editedChar--;
                            editingText[editingPosition] = editedChar;
                        }
                    }
                    else if (editingPosition == EXIT_CHAR_POSITION)
                    {
                        if (approving == EndEditEnum.Approving) approving = EndEditEnum.Cancelling;
                        else if (approving == EndEditEnum.Cancelling) approving = EndEditEnum.Approving;
                    }
                    break;

                case FEZ_Shields.KeypadLCD.Keys.Select:
                    if (editingPosition == EXIT_CHAR_POSITION)
                    {
                        if (approving == EndEditEnum.Approving)
                        {
                            string strHour = new string(editingText, 0, 2);
                            string strMinute = new string(editingText, 2, 2);
                            string strSecond = new string(editingText, 4, 2);
                            
                            int iHour = int.Parse(strHour);
                            int iMinute = int.Parse(strMinute);
                            int iSecond = int.Parse(strSecond);

                            Utility.SetLocalTime(new DateTime(2011, 01, 01, iHour, iMinute, iSecond, 0));
                        }
                        isEditing = false;
                        isInitialised = false;
                        approving = EndEditEnum.NotEditing;
                    }
                    break;
            }
        }

        public bool IsProcessingKey()
        {
            return isEditing;
        }

        public void BeginEdit()
        {
            isEditing = true;
            editingPosition = HOUR_POSITION_0;
            approving = EndEditEnum.Approving;
        }
        
        public bool CanEditLine()
        {
            return true;
        }
    }
}
