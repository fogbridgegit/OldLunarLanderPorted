using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WWI.Samples.LunarModule.Program
{
    public enum ProgramStateEnum
    {
        Initializing,
        StartupScreen,
        ChooseRoom,
        ScanOrLoadRoom,
        ScanRoom,
        LoadingScan,
        SavingScan,
        LandingPadPlacement,
        ControlsDisplay,
        Gameplay,
        GameplayFinished,
        Quitting,
    }

}
