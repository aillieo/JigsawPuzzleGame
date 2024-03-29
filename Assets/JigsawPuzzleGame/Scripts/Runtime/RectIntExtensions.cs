// -----------------------------------------------------------------------
// <copyright file="RectIntExtensions.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using UnityEngine;

namespace AillieoTech.Game
{
    public static class RectIntExtensions
    {
        public static Rect ToRectFloat(this RectInt rectInt)
        {
            return new Rect(rectInt.position, rectInt.size);
        }
    }
}
