//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace WWIToolkit.Unity.Input
{
    public class InputSourceUnityGamepad : InputSourceGamepadBase
    {
        // Variations based on platform
        public static readonly string[] SupportedGamepadNames = {
        "xbox"
    };
        /*  Temporarily disabling until wireless bluetooth controller situation is worked out
            "controller (xbox 360 for windows)",
            "controller (xbox one for windows)",
            "xbox 360 controller for windows",
            "xbox one controller for windows",
            "xbox controller for windows",
        };*/

        public const string ButtonA = "Xbox_A";
        public const string ButtonB = "Xbox_B";
        public const string ButtonX = "Xbox_X";
        public const string ButtonY = "Xbox_Y";
        public const string ButtonStart = "Xbox_MenuButton";
        public const string AxisLeftStickH = "Xbox_LeftStick_X";
        public const string AxisLeftStickV = "Xbox_LeftStick_Y";
        public const string AxisRightStickH = "Xbox_RightStick_X";
        public const string AxisRightStickV = "Xbox_RightStick_Y";
        public const string AxisDpadH = "Xbox_Dpad_X";
        public const string AxisDpadV = "Xbox_Dpad_Y";
        public const string TriggerLeft = "Xbox_LeftTrigger";
        public const string TriggerRight = "Xbox_RightTrigger";
        public const string TriggerShared = "Xbox_Trigger_Shared";

        bool present;

        public override bool IsPresent()
        {
            foreach (string joystickName in UnityEngine.Input.GetJoystickNames())
            {
                foreach (string supportedGamepad in SupportedGamepadNames)
                {
                    /*Temporarily disabling until wireless bluetooth controller situation is worked out
                    if (joystickName.ToLower() == supportedGamepad) {
                    */
                    if (joystickName.ToLower().Contains(supportedGamepad))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void _Update()
        {
            base._Update();

            if (IsPresent())
            {
                aButtonState.ApplyState(UnityEngine.Input.GetButton(ButtonA));
                bButtonState.ApplyState(UnityEngine.Input.GetButton(ButtonB));
                xButtonState.ApplyState(UnityEngine.Input.GetButton(ButtonX));
                yButtonState.ApplyState(UnityEngine.Input.GetButton(ButtonY));
                startButtonState.ApplyState(UnityEngine.Input.GetButton(ButtonStart));

                leftJoyVector = new Vector2(UnityEngine.Input.GetAxis(AxisLeftStickH), UnityEngine.Input.GetAxis(AxisLeftStickV));
                rightJoyVector = new Vector2(UnityEngine.Input.GetAxis(AxisRightStickH), UnityEngine.Input.GetAxis(AxisRightStickV));
                trigVector = new Vector2(UnityEngine.Input.GetAxis(TriggerLeft), UnityEngine.Input.GetAxis(TriggerRight));
                padVector = new Vector2(UnityEngine.Input.GetAxis(AxisDpadH), UnityEngine.Input.GetAxis(AxisDpadV));
            }
        }
    }
}