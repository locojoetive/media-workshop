using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CenterOfMassController : MonoBehaviour
{
    public Rigidbody2D rigidbody2D;
    void Awake()
    {
        rigidbody2D.centerOfMass = Vector2.zero;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(rigidbody2D.centerOfMass), 0.1f);
    }
}
