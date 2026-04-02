using System.Collections;
using UnityEngine;

public class MovementInfluenceController : MonoBehaviour
{
    public float movementInfluence = 1f;
    public bool isFading = false;
    public bool isStunned = false;

    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void FadeMovementForDuration(float duration)
    {
        if (isFading)
        {
            return;
        }
        isFading = true;
        StartCoroutine(FadeMovementForDurationCoroutine(duration));
    }

    private IEnumerator FadeMovementForDurationCoroutine(float duration)
    {
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var factor = AnimationHelper.EaseInQubic(elapsedTime / duration);
            movementInfluence = Mathf.Lerp(0f, 1f, factor);
            yield return null;
        }
        movementInfluence = 1f;
        isFading = false;
    }

    internal void Stun()
    {
        isStunned = true;
    }

    internal void Unstun()
    {
        isStunned = false;
    }

    public void SetForceToRigidbody(Vector2 force)
    {
        rb.linearVelocity = rb.linearVelocity * (1f - movementInfluence) + force * movementInfluence;
    }

    public Vector2 GetForcePlusLinearVelocity(Vector2 force)
    {
        return rb.linearVelocity + movementInfluence * force;
    }
}
