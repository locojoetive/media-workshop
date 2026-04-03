using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public bool isNeutralized = false;
    public bool causedDamage = false;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    
    private void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        col.enabled = true;
        spriteRenderer.color = Color.red;
        rb.linearVelocity = Vector3.down;
    }

    internal void Initialize(float projectileSpeed)
    {
        Initialize();
        rb.linearVelocity = Vector3.down * projectileSpeed;
    }
    
    internal void Initialize(Vector3 projectileShootDirection)
    {
        Initialize();
        rb.linearVelocity = projectileShootDirection;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (causedDamage)
        {
            return;
        }

        if (isNeutralized)
        {
            if (collision.collider.TryGetComponent<HittableController>(out var hittable))
            {
                hittable.TakeDamage();
                causedDamage = true;
                var normal = collision.GetContact(0).normal;
                rb.linearVelocity = -normal.normalized * rb.linearVelocity.magnitude * 0.5f;
                transform.localScale = transform.localScale * 0.5f;
                col.enabled = false;
                StartCoroutine(DissolveCoroutine());
                return;
            }
        }

        if (collision.collider.TryGetComponent<TrampolineController>(out var _))
        {
            return;
        }
        if (collision.collider.TryGetComponent<PlayerController>(out var playerController))
        {
            causedDamage = true;
            var normal = collision.GetContact(0).normal;
            rb.linearVelocity = -normal.normalized * rb.linearVelocity.magnitude * 0.5f;
            transform.localScale = transform.localScale * 0.5f;
            col.enabled = false;
            playerController.TakeDamage(normal);
            StartCoroutine(DissolveCoroutine());
            return;
        }
        isNeutralized = true;
        spriteRenderer.color = Color.green;
        Destroy(gameObject, 5f);
    }

    private IEnumerator DissolveCoroutine()
    {
        var color = spriteRenderer.color;
        var dissolveDuration = 0.3f;
        var elapsedTime = 0f;
        while (elapsedTime < dissolveDuration)
        {
            color.a = 1f - elapsedTime / dissolveDuration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
