// -----------------------------------------------------------------------
// <copyright file="Test.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    public class Test : MonoBehaviour
    {
        [SerializeField]
        private Texture2D sourceTexture;
        [SerializeField]
        private Texture2D targetTexture;

        public Vector2Int dimensions;

        public int pieceIndexA;
        public int pieceIndexB;

        [ContextMenu("Cut")]
        public async void Cut()
        {
            var context = new CuttingContext(new Vector2Int(this.sourceTexture.width, this.sourceTexture.height), this.dimensions);
            var pieceDataA = await PieceCreator.CalculatePieceData(context, this.pieceIndexA);
            var pieceDataB = await PieceCreator.CalculatePieceData(context, this.pieceIndexB);

            Color32[] sourcePixels = this.sourceTexture.GetPixels32();
            Color32[] targetPixels = this.targetTexture.GetPixels32();

            var width = this.sourceTexture.width;
            var height = this.sourceTexture.height;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var index = y * width + x;
                    targetPixels[index] = Color.clear;
                }
            }

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var index = y * width + x;
                    var point = new Vector2Int(x, y);

                    byte alpha = 0;
                    var color = sourcePixels[index];
                    foreach (var pieceData in new PieceData[] { pieceDataA, pieceDataB })
                    {
                        if (pieceData.extendedRect.Contains(point))
                        {
                            var px = x - pieceData.extendedRect.x;
                            var py = y - pieceData.extendedRect.y;
                            var pIndex = px + py * pieceData.extendedRect.width;
                            alpha = (byte)Mathf.Max(pieceData.mask[pIndex], alpha);

                            if (pieceData.border[pIndex] > 0)
                            {
                                color = Color.black;
                                alpha = 255;
                            }
                        }
                    }

                    color.a = alpha;
                    targetPixels[index] = color;
                }
            }

            this.targetTexture.SetPixels32(targetPixels);
            this.targetTexture.Apply();
            UnityEditor.EditorUtility.SetDirty(this.targetTexture);
        }

        public float SDF(Vector2 point)
        {
            return SDFUtils.Circle(point, Vector2.zero, 120f);
        }
    }
}
