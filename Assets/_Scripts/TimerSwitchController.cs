using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class TimerSwitchController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color active;
    public Color inactive;
    public bool isActive = false;
    public float activeDuration;
    public int toggleCount;
    public UnityEvent onSwitchActive;
    public UnityEvent onSwitchInactive;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = inactive;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive)
        {
            return;
        }
        Debug.Log("Something entered the Timer Switch");

        if (!other.TryGetComponent<PlayerController>(out var _)
            && !other.TryGetComponent<BatController>(out var _)
            && !other.TryGetComponent<ProjectileController>(out var _)
        )
        { 
            return;
        }

        Debug.Log("Activate Timer Switch");
        isActive = true;
        StartCoroutine(DeactivateAfterTimeLimit());
    }

    private IEnumerator DeactivateAfterTimeLimit()
    {
        onSwitchActive.Invoke();
        Debug.Log("onswitchactive invoked");
        spriteRenderer.color = active;

        float elapsedTime = 0f;
        float toggleTime = 0f;
        float toggleAfterSeconds = 0.125f * activeDuration;
        var toggle = true;
        while (elapsedTime < activeDuration)
        {
            elapsedTime += Time.deltaTime;
            toggleTime += Time.deltaTime;
            if (toggleTime >= toggleAfterSeconds)
            {
                toggleTime = 0f;
                toggle = !toggle;
                toggleAfterSeconds = 0.125f * (activeDuration - elapsedTime);
                spriteRenderer.color = toggle ? active : inactive;
            }
            yield return null;
        }
        onSwitchInactive.Invoke();
        spriteRenderer.color = inactive;
        isActive = false;
    }
}
