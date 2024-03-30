// -----------------------------------------------------------------------
// <copyright file="Board.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    public class Board : MonoBehaviour
    {
        public Texture2D image;
        public Vector2Int dimension;

        private Piece[] managedPieces;

        [ContextMenu("Generate")]
        private async void Generate()
        {
            var size = new Vector2Int(this.image.width, this.image.height);
            var pieceCount = this.dimension.x * this.dimension.y;
            this.managedPieces = new Piece[pieceCount];
            var context = new CuttingContext(size, this.dimension);
            for (var i = 0; i < pieceCount; i++)
            {
                var pieceData = await PieceCreator.CalculatePieceData(context, i);
                var pieceObject = Piece.Create(this.image, pieceData);
                this.managedPieces[i] = pieceObject;
                pieceObject.transform.localPosition = (Vector3)(Vector2)pieceData.extendedRect.position / 100f;
            }
        }
    }
}
