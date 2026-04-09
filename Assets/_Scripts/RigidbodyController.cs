using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RigidbodyController : MonoBehaviour
{
    private float movementInfluence = 1f;
    public bool isFading = false;
    public bool isStunned = false;

    public Rigidbody2D rb;
    public Vector2 Position => rb.position;
    public float LinearVelocityX => rb.linearVelocity.x;
    public float LinearVelocityY => rb.linearVelocity.y;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = rb.linearVelocity * (1f - movementInfluence) + velocity * movementInfluence;
    }

    public void SetVelocityX(float horizontalVelocity)
    {
        var speed = horizontalVelocity * movementInfluence + rb.linearVelocity.x * (1f - movementInfluence);
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
    }
    public void SetVelocityY(float verticalVelocity)
    {
        var speed = verticalVelocity * movementInfluence + rb.linearVelocity.y * (1f - movementInfluence);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, speed);
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

    internal void FreeFromConstraints()
    {
        rb.constraints = RigidbodyConstraints2D.None;
    }

    internal void SetVelocityInRespectToMass(Vector2 velocity)
    {
        var velocityInRespectToMass = velocity / (rb.mass * rb.mass);
        rb.linearVelocity = rb.linearVelocity * (1f - movementInfluence) + velocityInRespectToMass * movementInfluence;
    }
}
