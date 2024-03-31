// -----------------------------------------------------------------------
// <copyright file="DragEvents.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using UnityEngine;

    public class DragEvents : MonoBehaviour
    {
        private bool isPressed;
        private bool isDragging;
        private Vector2 lastPosition;

        public event Action<Vector2> OnDragBegin;

        public event Action<Vector2> OnDrag;

        public event Action<Vector2> OnDragEnd;

        private void OnEnable()
        {
            Input.simulateMouseWithTouches = true;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.isPressed = true;
            }
            else if (Input.GetMouseButton(0) && this.isPressed)
            {
                Vector2 currentPosition = Input.mousePosition;

                if (Vector2.SqrMagnitude(currentPosition - this.lastPosition) > 1f)
                {
                    if (!this.isDragging)
                    {
                        this.isDragging = true;
                        this.OnDragBegin?.Invoke(currentPosition);
                    }
                    else
                    {
                        this.OnDrag?.Invoke(currentPosition);
                    }

                    this.lastPosition = currentPosition;
                }
            }
            else if (Input.GetMouseButtonUp(0) && this.isDragging)
            {
                Vector2 endPosition = Input.mousePosition;
                this.OnDragEnd?.Invoke(endPosition);
                this.isDragging = false;
                this.isPressed = false;
            }
        }
    }
}
