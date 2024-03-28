// -----------------------------------------------------------------------
// <copyright file="EdgeData.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    public readonly struct EdgeData
    {
        public readonly EdgePosition position;
        public readonly EdgeShape shape;

        public EdgeData(EdgePosition position, EdgeShape shape)
        {
            this.position = position;
            this.shape = shape;
        }
    }
}
