// -----------------------------------------------------------------------
// <copyright file="Piece.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Piece : MonoBehaviour
    {
        internal PieceData pieceData;

        public static Piece Create(Texture2D image, PieceData pieceData)
        {
            var go = new GameObject($"piece_{pieceData.index}");
            var piece = go.AddComponent<Piece>();
            piece.pieceData = pieceData;

            var sprite = go.AddComponent<SpriteRenderer>();
            var pieceTexture = PieceCreator.CreateMaskedTexture(image, pieceData);
            sprite.sprite = PieceCreator.CreateSprite(pieceTexture);

            var list = new List<Vector2>();
            sprite.sprite.GetPhysicsShape(0, list);
            PolygonCollider2D collider = go.AddComponent<PolygonCollider2D>();
            collider.SetPath(0, list);

            return piece;
        }

        public void OnDragBegin(Vector2 screenPosition)
        {
        }

        public void OnDrag(Vector2 screenPosition)
        {
        }

        public void OnDragEnd(Vector2 screenPosition)
        {
        }
    }
}
