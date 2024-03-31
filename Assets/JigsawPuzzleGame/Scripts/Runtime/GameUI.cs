// -----------------------------------------------------------------------
// <copyright file="GameUI.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameUI : MonoBehaviour
    {
        public event Action onRestartClicked;

        public float gameStartTime;
        public float gemeEndTime;
        public bool isPlaying;

        private void OnGUI()
        {
            if (this.isPlaying)
            {
                var second = Time.time - this.gameStartTime;
                var timeStr = $"{(int)second / 60}:{second % 60:00}";
                GUI.Label(new Rect(10, 10, 100, 20), timeStr);
            }
            else
            {
                var second = this.gemeEndTime - this.gameStartTime;
                var timeStr = $"{(int)second / 60}:{second % 60:00}";
                GUI.Label(new Rect(10, 10, 100, 20), timeStr);
            }

            if (GUI.Button(new Rect(10, 40, 100, 20), "Start"))
            {
                this.onRestartClicked?.Invoke();
            }
        }
    }
}
