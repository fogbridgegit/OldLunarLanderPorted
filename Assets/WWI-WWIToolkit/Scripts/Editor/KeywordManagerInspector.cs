//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System.Collections.Generic;
using UnityEditor;
using WWIToolkit.Unity.Speech;

//using HUX.Speech;

namespace WWIToolkit.Unity.Editor
{
    [CustomEditor(typeof(KeywordManager))]
    public class KeywordManagerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (!EditorApplication.isPlaying)
                return;

            KeywordManager keywordManager = (KeywordManager)target;
            
            HUXEditorUtils.BeginSectionBox("Registered keywords");
            foreach (KeyValuePair<string,List<string>> command in keywordManager.EditorCommandDescriptions)
            {
                HUXEditorUtils.BeginSubSectionBox(command.Key);
                foreach (string commandTarget in command.Value)
                {
                    EditorGUILayout.LabelField(commandTarget, EditorStyles.wordWrappedLabel);
                }
                HUXEditorUtils.EndSubSectionBox();
            }
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.SaveChanges(target);
        }
    }
}