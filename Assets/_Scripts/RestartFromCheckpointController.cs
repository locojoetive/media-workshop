using UnityEngine;

public class RestartFromCheckpointController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<HittableController>(out var hittableController))
        {
            hittableController.Die();
            return;
        }

        if (collision.TryGetComponent<ProjectileController>(out var _))
        {
            Destroy(collision.gameObject);
            return;
        }
    }
}
