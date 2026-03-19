using System;
using UnityEngine;

public class WalkerSpitterProjectileController : MonoBehaviour
{
    private Rigidbody2D rb;
    internal void Initialize(Vector3 targetPosition, float projectileSpeed, float damage = 1f)
    {
        rb = GetComponent<Rigidbody2D>();
        var gravity = Physics2D.gravity.y * rb.gravityScale;
        var direction = CalculateProjectileDirection(targetPosition, projectileSpeed, gravity);
        Debug.DrawRay(transform.position, direction, Color.blue, 5f);
        rb.linearVelocity = direction;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (!collision.collider.TryGetComponent<PlayerController>(out var playerController))
        {
            Destroy(gameObject);
            return;
        }
        playerController.TakeDamage(collision.GetContact(0).normal);
        Destroy(gameObject, 1f);
    }

    // Ballistic trajectory calculation
    private Vector2 CalculateProjectileDirection(Vector3 targetPosition, float projectileSpeed, float gravity)
    {
        Vector2 displacement = targetPosition - transform.position;
        float dx = displacement.x;
        float dy = displacement.y;
        
        // Gravity must be positive magnitude for formula
        gravity = Mathf.Abs(gravity);
        
        float v2 = projectileSpeed * projectileSpeed;
        float horizontalDist = Mathf.Abs(dx);
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
}
