// -----------------------------------------------------------------------
// <copyright file="CuttingContext.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    public class CuttingContext
    {
        public readonly Vector2Int boardSize;
        public readonly Vector2Int dimension;

        internal readonly Vector2 pieceRawSize;
        internal readonly PieceData[] pieceData;

        public CuttingContext(Vector2Int boardSize, Vector2Int dimension)
        {
            this.boardSize = boardSize;
            this.dimension = dimension;

            var pieceWidth = boardSize.x / dimension.x;
            var pieceHeight = boardSize.y / dimension.y;
            this.pieceRawSize = new Vector2(pieceWidth, pieceHeight);

            this.pieceData = new PieceData[dimension.x * dimension.y];
        }
    }
}
