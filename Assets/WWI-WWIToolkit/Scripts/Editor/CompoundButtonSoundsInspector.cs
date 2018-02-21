//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using UnityEditor;
using UnityEngine;
using WWIToolkit.Unity.Buttons;

//using HUX.Buttons;

namespace WWIToolkit.Unity.Editor
{
    [CustomEditor(typeof(CompoundButtonSounds))]
    public class CompoundButtonSoundsInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            CompoundButtonSounds soundButton = (CompoundButtonSounds)target;

            GUI.color = Color.white;
            soundButton.SoundProfile = HUXEditorUtils.DrawProfileField<ButtonSoundProfile>(soundButton.SoundProfile);

            if (soundButton.SoundProfile == null)
            {
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            HUXEditorUtils.DrawProfileInspector(soundButton.SoundProfile, soundButton);

            HUXEditorUtils.SaveChanges(target, soundButton.SoundProfile);
        }
    }
}
