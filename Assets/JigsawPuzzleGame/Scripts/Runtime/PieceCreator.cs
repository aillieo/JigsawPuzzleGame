// -----------------------------------------------------------------------
// <copyright file="PieceCreator.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;

    public static class PieceCreator
    {
        private static readonly PieceData dummy = new PieceData(-1);

        public static Task<PieceData> CreatePiece(CuttingContext context, int index)
        {
            if (context.pieceData[index] != null)
            {
                return Task.FromResult(context.pieceData[index]);
            }

            // place holder
            context.pieceData[index] = dummy;

            var tcs = new TaskCompletionSource<PieceData>();

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    var piece = CreatePieceInternal(context, index);
                    context.pieceData[index] = piece;
                    tcs.SetResult(piece);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            return tcs.Task;
        }

        private static PieceData CreatePieceInternal(CuttingContext context, int index)
        {
            var pieceData = new PieceData(index);

            ConfigRect(pieceData, context);

            ConfigEdgeShape(pieceData, context);

            PopulateMask(pieceData, context);

            return pieceData;
        }

        private static void ConfigRect(PieceData piece, CuttingContext context)
        {
            var dimension = context.dimension;

            var index = piece.index;
            var x = index % dimension.x;
            var y = index / dimension.y;

            var width = context.pieceRawSize.x;
            var height = context.pieceRawSize.y;

            var startX = Mathf.RoundToInt(x * width);
            var startY = Mathf.RoundToInt(y * height);
            var nextStartX = Mathf.RoundToInt((x + 1) * width);
            var nextStartY = Mathf.RoundToInt((y + 1) * height);

            piece.rawRect = new RectInt(startX, startY, nextStartX - startX, nextStartY - startY);

            var extendedRect = piece.rawRect;
            var interlockingW = Mathf.RoundToInt(piece.rawRect.width * PieceShapeDefine.interlockingHeight);
            var interlockingH = Mathf.RoundToInt(piece.rawRect.height * PieceShapeDefine.interlockingHeight);

            extendedRect.x -= interlockingW;
            extendedRect.y -= interlockingH;
            extendedRect.width += 2 * interlockingW;
            extendedRect.height += 2 * interlockingH;

            piece.extendedRect = extendedRect;
        }

        private static void PopulateMask(PieceData piece, CuttingContext context)
        {
            var extendedRect = piece.extendedRect;
            var width = extendedRect.width;
            var height = extendedRect.height;
            piece.mask = new float[width * height];
            var xMin = extendedRect.xMin;
            var yMin = extendedRect.yMin;

            var sdfThresholdL = PieceShapeDefine.sdfThresholdL * piece.rawRect.width;
            var sdfThresholdH = PieceShapeDefine.sdfThresholdH * piece.rawRect.width;
            for (var x = xMin; x < extendedRect.xMax; x++)
            {
                for (var y = yMin; y < extendedRect.yMax; y++)
                {
                    var index = (x - xMin) + (y - yMin) * width;
                    var point = new Vector2Int(x, y);
                    var sdfValue = GetSDFValue(piece, point);
                    var alpha = 1 - SDFUtils.SmoothStep(sdfThresholdL, sdfThresholdH, sdfValue);
                    piece.mask[index] = alpha;
                }
            }
        }

        private static void ConfigEdgeShape(PieceData piece, CuttingContext context)
        {
            var dimension = context.dimension;

            var index = piece.index;
            var x = index % dimension.x;
            var y = index / dimension.y;

            piece.left = (x + y) % 2 == 0 ? EdgeShape.Notch : EdgeShape.Bulge;
            piece.right = piece.left;
            piece.top = (x + y) % 2 == 0 ? EdgeShape.Bulge : EdgeShape.Notch;
            piece.bottom = piece.top;

            if (x == 0)
            {
                piece.left = EdgeShape.Straight;
            }

            if (x == dimension.x - 1)
            {
                piece.right = EdgeShape.Straight;
            }

            if (y == 0)
            {
                piece.bottom = EdgeShape.Straight;
            }

            if (y == dimension.y - 1)
            {
                piece.top = EdgeShape.Straight;
            }
        }

        private static float GetSDFValue(PieceData piece, Vector2Int point)
        {
            // 中心区域
            var sdf = SDFUtils.Rectangle(point, piece.rawRect.center, piece.rawRect.width, piece.rawRect.height);

            var circleRadius = PieceShapeDefine.circleRadius * piece.rawRect.height;
            var blend = PieceShapeDefine.sdfBlendRadius * piece.rawRect.height;

            // 顶部
            if (piece.top != EdgeShape.Straight)
            {
                var circleCenter = GetCuttingCircleCenter(EdgePosition.Top, piece.top, piece.rawRect);
                var sdfCircle = SDFUtils.Circle(point, circleCenter, circleRadius);
                sdf = ModifySDFValueForEdge(sdf, sdfCircle, piece.top, blend);
            }

            // 底部
            if (piece.bottom != EdgeShape.Straight)
            {
                var circleCenter = GetCuttingCircleCenter(EdgePosition.Bottom, piece.bottom, piece.rawRect);
                var sdfCircle = SDFUtils.Circle(point, circleCenter, circleRadius);
                sdf = ModifySDFValueForEdge(sdf, sdfCircle, piece.bottom, blend);
            }

            // 左边
            if (piece.left != EdgeShape.Straight)
            {
                var circleCenter = GetCuttingCircleCenter(EdgePosition.Left, piece.left, piece.rawRect);
                var sdfCircle = SDFUtils.Circle(point, circleCenter, circleRadius);
                sdf = ModifySDFValueForEdge(sdf, sdfCircle, piece.left, blend);
            }

            // 右边
            if (piece.right != EdgeShape.Straight)
            {
                var circleCenter = GetCuttingCircleCenter(EdgePosition.Right, piece.right, piece.rawRect);
                var sdfCircle = SDFUtils.Circle(point, circleCenter, circleRadius);
                sdf = ModifySDFValueForEdge(sdf, sdfCircle, piece.right, blend);
            }

            return sdf;
        }

        private static Vector2 GetCuttingCircleCenter(EdgePosition edge, EdgeShape shape, RectInt rawRect)
        {
            var center = rawRect.center;
            var flip = shape == EdgeShape.Notch;

            var halfWidth = rawRect.width * 0.5f;
            var halfHeight = rawRect.height * 0.5f;

            var circleDirection = Vector2.zero;

            switch (edge)
            {
                case EdgePosition.Top:
                    center.y += halfHeight;
                    circleDirection.y = flip ? -1 : 1;
                    break;
                case EdgePosition.Bottom:
                    center.y -= halfHeight;
                    circleDirection.y = flip ? 1 : -1;
                    break;
                case EdgePosition.Left:
                    center.x -= halfWidth;
                    circleDirection.x = flip ? 1 : -1;
                    break;
                case EdgePosition.Right:
                    center.x += halfWidth;
                    circleDirection.x = flip ? -1 : 1;
                    break;
            }

            var circleDistX = PieceShapeDefine.circleToEdgeDistance * rawRect.width * circleDirection.x;
            var circleDistY = PieceShapeDefine.circleToEdgeDistance * rawRect.height * circleDirection.y;
            var circleDist = new Vector2(circleDistX, circleDistY);

            return center + circleDist;
        }

        private static float ModifySDFValueForEdge(float sdf, float sdfCircle, EdgeShape shape, float blend)
        {
            switch (shape)
            {
                case EdgeShape.Notch:
                    sdf = SDFUtils.SmoothMax(sdf, -sdfCircle, blend);
                    break;
                case EdgeShape.Bulge:
                    sdf = SDFUtils.SmoothMin(sdf, sdfCircle, blend);
                    break;
            }

            return sdf;
        }
    }
}
