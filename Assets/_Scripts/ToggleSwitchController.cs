using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class ToggleSwitchController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color active;
    public Color inactive;
    public bool isActive = false;
    public UnityEvent onSwitchActive;
    public UnityEvent onSwitchInactive;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isActive = false;
        spriteRenderer.color = inactive;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out var _)
            && !other.TryGetComponent<BatController>(out var _)
            && !other.TryGetComponent<ProjectileController>(out var _)
        )
        {
            return;
        }



        if (!isActive)
        {
            onSwitchActive.Invoke();
            spriteRenderer.color = active;
            isActive = false;
        }
        else
        {
            onSwitchInactive.Invoke();
            spriteRenderer.color = inactive;
            isActive = true;
        }
    }
}
