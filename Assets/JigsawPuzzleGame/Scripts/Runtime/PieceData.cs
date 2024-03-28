// -----------------------------------------------------------------------
// <copyright file="PieceData.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    public class PieceData
    {
        public readonly int index;

        public EdgeShape top;
        public EdgeShape bottom;
        public EdgeShape left;
        public EdgeShape right;

        public byte[] mask;
        public byte[] border;

        public RectInt rawRect;
        public RectInt extendedRect;

        public PieceData(int index)
        {
            this.index = index;
        }
    }
}
