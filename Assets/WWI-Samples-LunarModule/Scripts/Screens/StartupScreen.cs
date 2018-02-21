using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WWI.Samples.LunarModule;
using WWI.Samples.LunarModule.Lander;
using WWI.Samples.LunarModule.Program;
using WWIToolkit.Unity.Dialogs;
using WWIToolkit.Unity.Focus;
using WWIToolkit.Unity.Interaction;

namespace WWI.Samples.LunarModule.Screens
{
    public class StartupScreen : GameScreen
    {
          
        [SerializeField]
        private GameObject startupScreenMenuParent;

        [SerializeField]
        private GameObject startupMenu;

        [SerializeField]
        private GameObject difficultyMenu;

        public override void Activate(ProgramStateEnum state)
        {
            base.Activate(state);
            startupScreenMenuParent.gameObject.SetActive(true);
            startupMenu.SetActive(true);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            startupScreenMenuParent.SetActive(false);
            startupMenu.SetActive(false);
            difficultyMenu.SetActive(false);
        }



        protected override void OnFocusEnter(GameObject obj, FocusArgs args)
        {
            base.OnFocusEnter(obj, args);

            Debug.Log(obj.name + " : FocusEnter");

            switch (obj.name)
            {
                case "Easy":
                    difficultyMenu.GetComponent<SimpleInteractableMenuCollection>().SubtitleText = "Lots of fuel and a forgiving landing speed.";
                    break;

                case "Medium":
                    difficultyMenu.GetComponent<SimpleInteractableMenuCollection>().SubtitleText = "Moderate fuel and a challenging landing speed.";
                    break;

                case "Hard":
                    difficultyMenu.GetComponent<SimpleInteractableMenuCollection>().SubtitleText = "Very little fuel and no room for error!";
                    break;
            }
        }


        //protected override void FocusEnter(GameObject obj, PointerSpecificEventData eventData)
        //{
        //    eventData.Use();

        //    Debug.Log(obj.name + " : FocusEnter");

        //    switch (obj.name)
        //    {
        //        case "Easy":
        //            difficultyMenu.GetComponent<SimpleInteractableMenuCollection>().SubtitleText = "Lots of fuel and a forgiving landing speed.";
        //            break;

        //        case "Medium":
        //            difficultyMenu.GetComponent<SimpleInteractableMenuCollection>().SubtitleText = "Moderate fuel and a challenging landing speed.";
        //            break;

        //        case "Hard":
        //            difficultyMenu.GetComponent<SimpleInteractableMenuCollection>().SubtitleText = "Very little fuel and no room for error!";
        //            break;
        //    }



        //}

        protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            base.OnTapped(obj, eventArgs);

            Debug.Log(obj.name + " : InputDown");

            switch (obj.name)
            {
                case "Start":
                    Result = ProgramStateEnum.ChooseRoom;
                    Deactivate();
                    break;

                case "Difficulty":
                    // switch to difficulty menu
                    startupMenu.SetActive(false);
                    difficultyMenu.SetActive(true);
                    break;

                case "Quit":
                    Result = ProgramStateEnum.Quitting;
                    Deactivate();
                    Application.Quit();
                    break;

                case "Easy":
                    // switch back to main menu
                    startupMenu.SetActive(true);
                    difficultyMenu.SetActive(false);
                    LanderGameplay.Instance.Difficulty = LanderGameplay.DifficultyEnum.Easy;
                    break;

                case "Medium":
                    // switch back to main menu
                    startupMenu.SetActive(true);
                    difficultyMenu.SetActive(false);
                    LanderGameplay.Instance.Difficulty = LanderGameplay.DifficultyEnum.Medium;
                    break;

                case "Hard":
                    // switch back to main menu
                    startupMenu.SetActive(true);
                    difficultyMenu.SetActive(false);
                    LanderGameplay.Instance.Difficulty = LanderGameplay.DifficultyEnum.Hard;
                    break;

                default:
                    Debug.LogError("Unknown button choice in " + name + ": " + obj.name);
                    break;
            }
        }

        //protected override void InputDown(GameObject obj, InputEventData eventData)
        //{

        //    eventData.Use();

        //    Debug.Log(obj.name + " : InputDown");

        //    switch (obj.name)
        //    {
        //        case "Start":
        //            Result = ProgramStateEnum.ChooseRoom;
        //            Deactivate();
        //            break;

        //        case "Difficulty":
        //            // switch to difficulty menu
        //            startupMenu.SetActive(false);
        //            difficultyMenu.SetActive(true);
        //            break;

        //        case "Quit":
        //            Result = ProgramStateEnum.Quitting;
        //            Deactivate();
        //            Application.Quit();
        //            break;

        //        case "Easy":
        //            // switch back to main menu
        //            startupMenu.SetActive(true);
        //            difficultyMenu.SetActive(false);
        //            LanderGameplay.Instance.Difficulty = LanderGameplay.DifficultyEnum.Easy;
        //            break;

        //        case "Medium":
        //            // switch back to main menu
        //            startupMenu.SetActive(true);
        //            difficultyMenu.SetActive(false);
        //            LanderGameplay.Instance.Difficulty = LanderGameplay.DifficultyEnum.Medium;
        //            break;

        //        case "Hard":
        //            // switch back to main menu
        //            startupMenu.SetActive(true);
        //            difficultyMenu.SetActive(false);
        //            LanderGameplay.Instance.Difficulty = LanderGameplay.DifficultyEnum.Hard;
        //            break;

        //        default:
        //            Debug.LogError("Unknown button choice in " + name + ": " + obj.name);
        //            break;
        //    }



        //}

    }

}