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

        public Vector4 border;

        public float thresholdL = -10f;
        public float thresholdH = 10f;

        public int pieceIndexA;
        public int pieceIndexB;

        [ContextMenu("Cut")]
        public async void Cut()
        {
            var context = new CuttingContext(new Vector2Int(this.sourceTexture.width, this.sourceTexture.height), dimensions);
            var pieceDataA = await PieceCreator.CreatePiece(context, pieceIndexA);
            var pieceDataB = await PieceCreator.CreatePiece(context, pieceIndexB);

            Color32[] sourcePixels = sourceTexture.GetPixels32();
            Color32[] targetPixels = targetTexture.GetPixels32();

            var width = sourceTexture.width;
            var height = sourceTexture.height;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    //int index = y * width + x;
                    //targetPixels[index] = Color.clear;
                }
            }

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var index = y * width + x;
                    var point = new Vector2Int(x, y);

                    float alpha = 0;
                    foreach(var pieceData in new PieceData[] { pieceDataA, pieceDataB })
                    {
                        if (pieceData.extendedRect.Contains(point))
                        {
                            var px = x - pieceData.extendedRect.x;
                            var py = y - pieceData.extendedRect.y;
                            var pIndex = px + py * pieceData.extendedRect.width;
                            alpha = Mathf.Max(pieceData.mask[pIndex], alpha);
                        }
                    }

                    var color = sourcePixels[index];
                    //color.a = (byte)Mathf.Max(color.a, (byte)Mathf.RoundToInt((alpha * 255)));
                    color.a = (byte)Mathf.RoundToInt((alpha * 255));
                    targetPixels[index] = color;
                }
            }

            targetTexture.SetPixels32(targetPixels);
            targetTexture.Apply();
            UnityEditor.EditorUtility.SetDirty(targetTexture);
        }

        public float SDF(Vector2 point)
        {
            return SDFUtils.Circle(point, Vector2.zero, 120f);
        }
    }
}
