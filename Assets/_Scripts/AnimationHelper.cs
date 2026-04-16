using System;
using UnityEngine;

public static class AnimationHelper
{
    public static float EaseInQubic(float t)
    {
        return t * t * t;
    }

    public static float EaseOutQubic(float t)
    {
        return EaseInQubic(1f - t);
    }

    public static float EaseInOutCirc(float t)
    {
        return t < 0.5
            ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) / 2
            : (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) / 2;
    }

    internal static float EaseInQuint(float x)
    {
        return x * x * x * x * x;
    }

    internal static float EaseInSquare(float x)
    {
        return x * x;
    }

    internal static float EaseOutSquare(float x)
    {
        return EaseInSquare(1f - x);
    }

}
