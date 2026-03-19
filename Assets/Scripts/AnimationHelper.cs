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
}
