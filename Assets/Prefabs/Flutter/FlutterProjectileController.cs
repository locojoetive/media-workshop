using System;
using UnityEngine;

public class FlutterProjectileController : MonoBehaviour
{
    public bool isDestroyed = false;
    private Rigidbody2D rb;
    
    internal void Initialize(float projectileSpeed)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector3.down * projectileSpeed;
        Debug.DrawRay(transform.position, rb.linearVelocity, Color.blue, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDestroyed)
        {
            return;
        }

        if (collision.collider.TryGetComponent<TrampolineController>(out var trampolineController))
        {
            return;
        }
        if (!collision.collider.TryGetComponent<PlayerController>(out var playerController))
        {
            isDestroyed = true;
            Destroy(gameObject, 1f);
            return;
        }
        isDestroyed = true;
        playerController.TakeDamage(collision.GetContact(0).normal);
        Destroy(gameObject, 1f);
    }

}
