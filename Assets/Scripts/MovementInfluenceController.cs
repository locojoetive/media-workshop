using System;
using System.Collections;
using UnityEngine;

public class MovementInfluenceController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float movementInfluence = 1f;
    public float stunDuration = 0.5f;
    public bool isStunned = false;

    private Coroutine fadeMovementCoroutine;
    public void FadeMovementForBounceDuration(float duration, float horizontalImpact)
    {
        if (fadeMovementCoroutine != null)
        {
            StopCoroutine(fadeMovementCoroutine);
        }
        fadeMovementCoroutine = StartCoroutine(FadeMovementForBounceDurationCoroutine(duration, horizontalImpact));
    }

    private IEnumerator FadeMovementForBounceDurationCoroutine(float duration, float horizontalImpact)
    {
        var elapsedTime = duration * (1f - horizontalImpact);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var factor = AnimationHelper.EaseInQubic(elapsedTime / duration);
            movementInfluence = Mathf.Lerp(0f, 1f, factor);
            yield return null;
        }
        movementInfluence = 1f;
    }

    internal void Stun()
    {
        isStunned = true;
    }

    internal void Unstun()
    {
        isStunned = false;
    }
}
