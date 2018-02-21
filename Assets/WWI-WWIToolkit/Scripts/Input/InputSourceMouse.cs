//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using UnityEngine;


namespace WWIToolkit.Unity.Input
{
    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2,
        None = 3
    }

    public class InputSourceMouse : InputSourceBase
    {
        public Vector2 MousePos;
        public Vector2 LastMousePos;
        public float ScrollWheelDelta;
        private float m_PreviousWheelDelta;

        public event Action<MouseButton> OnMousePressed = delegate { };
        public event Action<MouseButton> OnMouseReleased = delegate { };
        public event Action<float> OnScrollWheelChanged = delegate { };

        public bool ButtonLeftPressed { get; private set; }
        public bool ButtonRightPressed { get; private set; }
        public bool ButtonMiddlePressed { get; private set; }

        private const float Tolerance = 0.0000001f;

        public void Start()
        {
            MousePos = UnityEngine.Input.mousePosition;
        }

        public override void _Update()
        {
            LastMousePos = MousePos;
            MousePos += new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
            m_PreviousWheelDelta = ScrollWheelDelta;
            ScrollWheelDelta = UnityEngine.Input.mouseScrollDelta.y;

            if (Math.Abs(ScrollWheelDelta - m_PreviousWheelDelta) > Tolerance)
            {
                OnScrollWheelChanged(ScrollWheelDelta);
            }

            HandleMousePressed();
            HandleMouseRelease();
            base._Update();
        }

        private void HandleMousePressed()
        {
            ButtonLeftPressed = UnityEngine.Input.GetMouseButton((int)MouseButton.Left);
            ButtonRightPressed = UnityEngine.Input.GetMouseButton((int)MouseButton.Right);
            ButtonMiddlePressed = UnityEngine.Input.GetMouseButton((int)MouseButton.Middle);

            if (UnityEngine.Input.GetMouseButtonDown((int)MouseButton.Left))
            {
                OnMousePressed(MouseButton.Left);
            }
            if (UnityEngine.Input.GetMouseButtonDown((int)MouseButton.Right))
            {
                OnMousePressed(MouseButton.Right);
            }
            if (UnityEngine.Input.GetMouseButtonDown((int)MouseButton.Middle))
            {
                OnMousePressed(MouseButton.Middle);
            }
        }

        private void HandleMouseRelease()
        {
            if (UnityEngine.Input.GetMouseButtonUp((int)MouseButton.Left))
            {
                OnMouseReleased(MouseButton.Left);
            }
            if (UnityEngine.Input.GetMouseButtonUp((int)MouseButton.Right))
            {
                OnMouseReleased(MouseButton.Right);
            }
            if (UnityEngine.Input.GetMouseButtonUp((int)MouseButton.Middle))
            {
                OnMouseReleased(MouseButton.Middle);
            }
        }
    }

    public class InputSourceMouseDisabled : InputSourceMouse
    {
        public new void Start()
        { }

        public override void _Update()
        { }
    }
}