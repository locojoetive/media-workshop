using System;

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
