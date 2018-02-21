//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEditor;
using UnityEngine;

namespace WWIToolkit.Unity.Editor
{
    /// <summary>
    /// Base class for profile inspectors
    /// Adds a 'target component' so inspectors can differentiate between local / global editing
    /// See compound button component inspectors for usage examples
    /// </summary>
    public class ProfileInspector : UnityEditor.Editor
    {
        public Component targetComponent;
    }
}