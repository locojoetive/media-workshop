using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ExplosionBehaviour : MonoBehaviour
{
    public float explosionForce;
    public CircleCollider2D circleCollider2D;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<TurnRigidbodyDynamicOnExplode>(out var turnRigidbodyDynamicOnExplode))
        {
            var explosionRadius = circleCollider2D.radius;
            var direction = turnRigidbodyDynamicOnExplode.transform.position - transform.position;
            var force = explosionForce * (direction.magnitude / explosionRadius) * direction.normalized; ;
            turnRigidbodyDynamicOnExplode.Explode(force);
        }
    }
}
