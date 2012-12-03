using System;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using Grommet.Ext;
using NetMf.CommonExtensions;

namespace BikeComputer
{
    class WheelSizeDisplayProcessor : IDisplayProcessor
    {

        enum EndEditEnum
        {
            Approving,
            Cancelling,
            NotEditing
        }

        const int WHEEL_SIZE_POSITION_0 = 0;
        const int WHEEL_SIZE_POSITION_1 = 1;
        const int WHEEL_SIZE_POSITION_2 = 2;
        const int EXIT_CHAR_POSITION = 3;
        
        private BikeData bikeData = null;
        private bool isInitialised = false;
        private bool isEditing = false;
        private char[] editingText = new char[3];
        private int editingPosition = WHEEL_SIZE_POSITION_0;
        private EndEditEnum approving = EndEditEnum.NotEditing;
        private bool blink = false;
        
        public WheelSizeDisplayProcessor(BikeData inBikeData)
        {
            bikeData = inBikeData;
        }

        private void Initialise()
        {
            if (!isInitialised)
            {
                if (bikeData.WheelSize > 999)
                {
                    bikeData.WheelSize = 999;
                }

                string strValue = bikeData.WheelSize.ToString();
                editingText[WHEEL_SIZE_POSITION_0] = strValue[WHEEL_SIZE_POSITION_0];
                editingText[WHEEL_SIZE_POSITION_1] = strValue[WHEEL_SIZE_POSITION_1];
                editingText[WHEEL_SIZE_POSITION_2] = strValue[WHEEL_SIZE_POSITION_2];

                isInitialised = true;
            }
        }

        public void Display()
        {

            Initialise();
                        
            FEZ_Shields.KeypadLCD.SetCursor(1, 0);
            FEZ_Shields.KeypadLCD.Print("Wheel size:");

            FEZ_Shields.KeypadLCD.SetCursor(1, 12);
            char[] prepareText = new char[3];
            prepareText[WHEEL_SIZE_POSITION_0] = editingText[WHEEL_SIZE_POSITION_0];
            prepareText[WHEEL_SIZE_POSITION_1] = editingText[WHEEL_SIZE_POSITION_1];
            prepareText[WHEEL_SIZE_POSITION_2] = editingText[WHEEL_SIZE_POSITION_2];
            if (isEditing && blink && (editingPosition >= WHEEL_SIZE_POSITION_0 && editingPosition <= WHEEL_SIZE_POSITION_2))
            {
                prepareText[editingPosition] = ' ';
            }
            string strValue = new string(prepareText);
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
                    if (editingPosition < WHEEL_SIZE_POSITION_0)
                    {
                        editingPosition = WHEEL_SIZE_POSITION_0;
                    }
                    break;

                case FEZ_Shields.KeypadLCD.Keys.Up:
                    if (editingPosition >= WHEEL_SIZE_POSITION_0 && editingPosition <= WHEEL_SIZE_POSITION_2)
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
                    if (editingPosition >= WHEEL_SIZE_POSITION_0 && editingPosition <= WHEEL_SIZE_POSITION_2)
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
                            string newValue = new string(editingText);
                            bikeData.WheelSize = int.Parse(newValue);
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
            editingPosition = WHEEL_SIZE_POSITION_0;
            approving = EndEditEnum.Approving;
        }
        
        public bool CanEditLine()
        {
            return true;
        }
    }
}
