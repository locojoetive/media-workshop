using System.Collections;
using UnityEngine;

public class TrampolineController : MonoBehaviour
{   
    [Header("Bounce Settings")]
    public float upBounceForce = 10f;
    public float horizontalBounceForce = 5f;
    public float releaseDuration = 0.1f;
    public float maxScale = 1.2f;
    public float fadeMovementDuration = 1f;

    [Header("Debugging")]
    public Vector2 originalScale;
    private Coroutine bounceCoroutine;
    public bool isResetting;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent<Rigidbody2D>(out var rigidbody)
            || rigidbody.bodyType != RigidbodyType2D.Dynamic
        ) {
            return;
        }

        // Apply bounce force
        // var normal = -collision.GetContact(0).normal;
        // var bounceDirection = new Vector2(
        //     normal.x,
        //     normal.y < 0 ? 0f : normal.y
        // ).normalized;
        // rigidbody.linearVelocity = 1f / (rigidbody.mass * rigidbody.mass) * new Vector2(
        //     bounceDirection.x * horizontalBounceForce,
        //     bounceDirection.y * upBounceForce
        // );
        // rigidbody.linearVelocity = Vector2.ClampMagnitude(rigidbody.linearVelocity, upBounceForce);
        
        
        var normal = -collision.GetContact(0).normal;
        var bounceDirection = normal.normalized;
        rigidbody.linearVelocity = 1f / (rigidbody.mass * rigidbody.mass) * bounceDirection * upBounceForce;

        if (collision.gameObject.TryGetComponent<MovementInfluenceController>(out var movementInfluenceController))
        {
            // Fade movement based on horizontal bounce force
            var horizontalImpact = AnimationHelper.EaseInQubic(Mathf.Abs(bounceDirection.x));
            Debug.DrawRay(
                collision.GetContact(0).point,
                bounceDirection,
                Color.red, 1f
            );
            movementInfluenceController.FadeMovementForBounceDuration(fadeMovementDuration, horizontalImpact);
        }
        
        // Animate
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
            transform.localScale = originalScale;
        }
        bounceCoroutine = StartCoroutine(Bounce());
    }

    private IEnumerator Bounce()
    {
        var requiredReleaseDuration = releaseDuration * (1f - (transform.localScale.y - 1f) / (maxScale - 1f));
        var halfReleaseDuration = requiredReleaseDuration / 2f;
        var elapsedTime = 0f;
        
        // Expand phase
        while (elapsedTime < halfReleaseDuration)
        {
            float factor = elapsedTime / halfReleaseDuration;
            float scale = Mathf.Lerp(1f, maxScale, factor);
            transform.localScale = new Vector3(scale, scale, 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Return phase
        elapsedTime = 0f;
        while (elapsedTime < halfReleaseDuration)
        {
            float factor = elapsedTime / halfReleaseDuration;
            float scale = Mathf.Lerp(originalScale.y * maxScale, originalScale.y, factor);
            transform.localScale = new Vector3(scale, scale, 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = new Vector3(originalScale.x, originalScale.y, 1f);
    }
}
