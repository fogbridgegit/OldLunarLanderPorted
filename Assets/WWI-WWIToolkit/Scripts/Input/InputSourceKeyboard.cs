//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;


namespace WWIToolkit.Unity.Input
{
    public class InputSourceKeyboard : InputSourceBase
    {


        public static bool GetKey(KeyCode keyCode)
        {
            return UnityEngine.Input.GetKey(keyCode);
        }

        public static bool GetKeyDown(KeyCode keyCode)
        {
            return UnityEngine.Input.GetKeyDown(keyCode);
        }

        public static bool GetKeyUp(KeyCode keyCode)
        {
            return UnityEngine.Input.GetKeyUp(keyCode);
        }
    }

}
