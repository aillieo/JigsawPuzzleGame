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
        private PieceData pieceData;

        public static Piece Create(Texture2D image, PieceData pieceData)
        {
            GameObject go = new GameObject($"piece_{pieceData.index}");
            var piece = go.AddComponent<Piece>();

            var sprite = go.AddComponent<SpriteRenderer>();
            sprite.sprite = PieceCreator.CreateSprite(image, pieceData);
            sprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            var mask = go.AddComponent<SpriteMask>();
            var maskTexture = PieceCreator.CreateMaskTexture(pieceData);
            mask.sprite = PieceCreator.CreateSprite(maskTexture);

            var list = new List<Vector2>();
            mask.sprite.GetPhysicsShape(0, list);
            PolygonCollider2D collider = go.AddComponent<PolygonCollider2D>();
            collider.SetPath(0, list);

            return piece;
        }
    }
}
