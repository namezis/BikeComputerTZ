using System;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using Grommet.Ext;
using NetMf.CommonExtensions;

namespace BikeComputer
{

    interface IDisplayProcessor
    {
        void Display();
        bool IsProcessingKey();
        void ProcessKey(FEZ_Shields.KeypadLCD.Keys keyPressed);
        void BeginEdit();
        bool CanEditLine();
    }

}
