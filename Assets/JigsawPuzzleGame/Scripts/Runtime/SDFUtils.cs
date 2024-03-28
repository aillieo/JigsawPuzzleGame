// -----------------------------------------------------------------------
// <copyright file="SDFUtils.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    public static class SDFUtils
    {
        public static float Circle(Vector2 point, Vector2 center, float radius)
        {
            var distance = Vector2.Distance(center, point) - radius;
            return distance;
        }

        public static float Rectangle(Vector2 point, Vector2 center, float width, float height)
        {
            var halfWidth = width / 2;
            var halfHeight = height / 2;

            var distanceX = Mathf.Abs(center.x - point.x) - halfWidth;
            var distanceY = Mathf.Abs(center.y - point.y) - halfHeight;

            var distance = Mathf.Max(distanceX, distanceY);
            return distance;
        }

        public static float SmoothStep(float edge0, float edge1, float x)
        {
            var t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
            return t * t * (3f - 2f * t);
        }

        public static float SmoothMin(float a, float b, float k)
        {
            var h = Mathf.Max(k - Mathf.Abs(a - b), 0.0f) / k;
            return Mathf.Min(a, b) - h * h * k * 0.25f;
        }

        public static float SmoothMax(float a, float b, float k)
        {
            var h = Mathf.Max(k - Mathf.Abs(a - b), 0.0f) / k;
            return Mathf.Max(a, b) + h * h * k * 0.25f;
        }
    }
}
