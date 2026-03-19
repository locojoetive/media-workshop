using UnityEngine;

public class HazardController : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        var playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            var collisionNormal = collision.GetContact(0).normal;
            playerController.TakeDamage(collisionNormal);
        }
    }
}
