using UnityEngine;

public class HazardController : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<HittableController>(out var hittableController))
        {
            var collisionNormal = collision.GetContact(0).normal;
            hittableController.TakeDamage(collisionNormal);
        }
    }
}
