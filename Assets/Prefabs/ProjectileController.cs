using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class ProjectileController : MonoBehaviour
{
    public bool canCauseDamage = false;
    public bool isDisabled = false;
    public float lifetime = 5f;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    private Coroutine lifetimeCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        col.enabled = true;
        canCauseDamage = true;
        isDisabled = false;
        spriteRenderer.color = Color.red;
        lifetimeCoroutine = StartCoroutine(LifetimeCoroutine());
    }

    internal void Initialize(float projectileSpeed)
    {
        rb.linearVelocity = Vector3.down * projectileSpeed;
    }
    
    internal void Initialize(Vector3 projectileShootDirection)
    {
        rb.linearVelocity = projectileShootDirection;
        rb.angularVelocity = Random.Range(-200f, 200f);
    }

    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(lifetime-3f);

        var waitForPointTwoSeconds = new WaitForSeconds(0.2f);
        for (int i = 0; i < 3; i++)
        {
            var color = spriteRenderer.color;
            color.a = 0.5f;
            spriteRenderer.color = color;
            yield return waitForPointTwoSeconds;
            color.a = 1f;
            spriteRenderer.color = color;
            yield return waitForPointTwoSeconds;
            yield return waitForPointTwoSeconds;
        }
        
        for (int i = 0; i < 3; i++)
        {
            var color = spriteRenderer.color;
            color.a = 0.5f;
            spriteRenderer.color = color;
            yield return waitForPointTwoSeconds;
            color.a = 1f;
            spriteRenderer.color = color;
            yield return waitForPointTwoSeconds;
        }
        
        var waitForPointOneSeconds = new WaitForSeconds(0.1f);
        for (int i = 0; i < 3; i++)
        {
            var color = spriteRenderer.color;
            color.a = 0.5f;
            spriteRenderer.color = color;
            yield return waitForPointOneSeconds;
            color.a = 1f;
            spriteRenderer.color = color;
            yield return waitForPointOneSeconds;
        }

        Disable(Vector2.zero);
    }

    private void Update()
    {
        if (isDisabled)
        {
            return;
        }
        if (rb.linearVelocity.magnitude < 1f && canCauseDamage)
        {
            canCauseDamage = false;
            spriteRenderer.color = Color.gray;
        }
        else if (rb.linearVelocity.magnitude >= 1f && !canCauseDamage)
        {
            canCauseDamage = true;
            spriteRenderer.color = Color.red;
        }
    }

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDisabled)
        {
            return;
        }

        if (!canCauseDamage)
        {
            return;
        }

        if (collision.collider.TryGetComponent<TrampolineController>(out var _))
        {
            return;
        }
        else if (collision.collider.TryGetComponent<HittableController>(out var hittable))
        {
            hittable.TakeDamage();
            Disable(collision.GetContact(0).normal);
            return;
        }
        else if (collision.collider.TryGetComponent<PlayerController>(out var playerController))
        {
            var normal = collision.GetContact(0).normal;
            playerController.TakeDamage(normal);
            Disable(normal);
            return;
        }
    }


    private void Disable(Vector2 normal)
    {
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
        }
        isDisabled = true;
        rb.linearVelocity = -normal.normalized * rb.linearVelocity.magnitude * 0.5f;
        transform.localScale = transform.localScale * 0.5f;
        col.enabled = false;
        spriteRenderer.color = Color.gray;
        StartCoroutine(DissolveCoroutine());
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
