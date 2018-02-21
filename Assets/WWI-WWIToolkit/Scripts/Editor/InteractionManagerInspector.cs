//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WWIToolkit.Unity.Interaction;

//using HUX.Interaction;

namespace WWIToolkit.Unity.Editor
{
    [CustomEditor(typeof(InteractionManager))]
    public class InteractionManagerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            InteractionManager interactionManager = (InteractionManager)target;

            interactionManager.RecognizableGesures = (UnityEngine.XR.WSA.Input.GestureSettings)HUXEditorUtils.EnumCheckboxField<UnityEngine.XR.WSA.Input.GestureSettings>(
                "Recognizable gestures",
                interactionManager.RecognizableGesures,
                "Default",
                UnityEngine.XR.WSA.Input.GestureSettings.Tap |
                UnityEngine.XR.WSA.Input.GestureSettings.DoubleTap |
                UnityEngine.XR.WSA.Input.GestureSettings.Hold |
                UnityEngine.XR.WSA.Input.GestureSettings.NavigationX |
                UnityEngine.XR.WSA.Input.GestureSettings.NavigationY);

            HUXEditorUtils.SaveChanges(target);
        }
    }
}
