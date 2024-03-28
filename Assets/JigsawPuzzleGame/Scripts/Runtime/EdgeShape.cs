// -----------------------------------------------------------------------
// <copyright file="EdgeShape.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    public enum EdgeShape : byte
    {
        Straight = 0,
        Notch,
        Bulge
    }

    public enum EdgePosition : byte
    {
        Top,
        Bottom,
        Left,
        Right,
    }
}
