using UnityEngine;

public static class MathHelper
{
    // Ballistic trajectory calculation
    public static Vector2 CalculateProjectileDirection(
        Vector3 projectileStartPosition,
        Vector3 targetPosition,
        float projectileSpeed,
        float gravity
    ) {
        Vector2 displacement = targetPosition - projectileStartPosition;
        float dx = displacement.x;
        float dy = displacement.y;
        
        // Gravity must be positive magnitude for formula
        gravity = Mathf.Abs(gravity);
        
        float v2 = projectileSpeed * projectileSpeed;
        float horizontalDist = Mathf.Abs(dx);        
        // Handle case where target is directly above or below
        if (Mathf.Approximately(horizontalDist, 0))
        {
            return Mathf.Sign(dy) * projectileSpeed * Vector2.up;
        }
        float discriminant = v2 * v2 - gravity * (gravity * horizontalDist * horizontalDist + 2 * v2 * dy);
        
        if (discriminant < 0)
        {
            Debug.LogWarning($"Target unreachable! Max range: {v2 / gravity}");
            return displacement.normalized * projectileSpeed;
        }
        
        // Calculate launch angle
        float tanTheta = (v2 - Mathf.Sqrt(discriminant)) / (gravity * horizontalDist);
        float angle = Mathf.Atan(tanTheta);
        
        // Adjust for direction (left vs right)
        if (dx < 0)
        {
            angle = Mathf.PI - angle;
        }
        
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * projectileSpeed;
    }

    public static float ClampAndMap(float value, float inMin, float inMax, float outMin, float outMax)
    {
        value = Mathf.Clamp(value, inMin, inMax);
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
