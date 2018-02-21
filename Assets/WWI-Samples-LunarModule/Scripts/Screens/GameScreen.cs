//using HoloToolkit.Unity;
//using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  WWI.Samples.LunarModule.Program;
using WWIToolkit.Unity.Receivers;
//using WWIToolkit.Unity.Utility;

namespace WWI.Samples.LunarModule.Screens
{

    public abstract class GameScreen : InteractionReceiver
    {

        //NOTE:  leverages holotoolkit
        [SerializeField]
        protected HeadsUpDirectionIndicator directionIndicator;

        [SerializeField]
        private GameObject headsUpTarget;


        private ProgramStateEnum gameScreenResult = ProgramStateEnum.Initializing;
        private bool isActive = false;


        public bool IsActive
        {
            get
            {
                return isActive;
            }
        }



        public virtual void Activate(ProgramStateEnum state)
        {
            isActive = true;
            gameScreenResult = state;
            directionIndicator.TargetObject = headsUpTarget;
        }

        public virtual void Deactivate()
        {
            isActive = false;
            directionIndicator.TargetObject = null;
        }

        protected virtual void Start()
        {
            Deactivate();
        }

        public ProgramStateEnum Result
        {
            get
            {
                return gameScreenResult;
            }
            protected set
            {
                gameScreenResult = value;
            }
        }

 
    }

}