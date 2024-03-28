// -----------------------------------------------------------------------
// <copyright file="PieceShapeDefine.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    public static class PieceShapeDefine
    {
        public static readonly float circleToEdgeDistance = 0.18f;
        public static readonly float circleRadius = 0.19f;

        public static readonly float interlockingHeight = 0.5f;

        public static readonly float borderSDFThreshold = 0.02f;

        public static readonly float sdfThresholdL = -0.01f;
        public static readonly float sdfThresholdH = 0.01f;
        public static readonly float sdfBlendRadius = 0.16f;
    }
}
