using Microsoft.Xna.Framework;

namespace Arena.Game.MathUtils;

public static class MathHelpers
{
    // Distance between two points
    public static float Distance(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b);
    }

    // Squared distance (faster, no sqrt)
    public static float DistanceSquared(Vector2 a, Vector2 b)
    {
        return Vector2.DistanceSquared(a, b);
    }

    // Dot product wrapper
    public static float Dot(Vector2 a, Vector2 b)
    {
        return Vector2.Dot(a, b);
    }

    // Cross product (2D scalar cross)
    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    // Linear interpolation
    public static float Lerp(float a, float b, float t)
    {
        return MathHelper.Lerp(a, b, t);
    }

    public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, t);
    }

    // Clamp value between min and max
    public static float Clamp(float value, float min, float max)
    {
        return MathHelper.Clamp(value, min, max);
    }
}