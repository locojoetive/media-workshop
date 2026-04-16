using UnityEngine;

public class ThrowRigidbodyController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if (collision.gameObject.TryGetComponent<RigidbodyController>(out var rigidbody))
        // {
        //     rigidbody.FadeMovementForDuration(0.5f);
        // }
    }
}
