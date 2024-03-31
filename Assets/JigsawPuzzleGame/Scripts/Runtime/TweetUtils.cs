// -----------------------------------------------------------------------
// <copyright file="TweetUtils.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using System.Collections;
    using UnityEngine;

    public static class TweetUtils
    {
        public static IEnumerator Move(GameObject gameObject, Vector3 targetWorldPosition, float time)
        {
            var currentPosition = gameObject.transform.position;
            var distance = targetWorldPosition - currentPosition;
            var speed = distance.magnitude / time;
            var direction = distance.normalized;
            var passedTime = 0f;
            while (passedTime < time)
            {
                passedTime += Time.deltaTime;
                gameObject.transform.position = currentPosition + direction * speed * passedTime;
                yield return null;
            }
        }
    }
}
