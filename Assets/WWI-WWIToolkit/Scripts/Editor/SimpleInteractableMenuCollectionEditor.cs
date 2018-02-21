
using UnityEngine;
using UnityEditor;
using HoloToolkit.Unity.Collections;
using WWIToolkit.Unity.Dialogs;

namespace WWIToolkit.Unity.Editor
{

    [CustomEditor(typeof(SimpleInteractableMenuCollection))]
    public class SimpleInteractableMenuCollectionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default
            base.OnInspectorGUI();

            // Place the button at the bottom
            SimpleInteractableMenuCollection myScript = (SimpleInteractableMenuCollection)target;
            if (GUILayout.Button("Update SimpleInteractableMenuCollection"))
            {
                myScript.UpdateCollection();
            }
        }
    }
}