using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class StaySwitchController : MonoBehaviour
{
    public List<Collider2D> stayingColliders;
    public Color active;
    public Color inactive;
    public bool isActive = true;
    public UnityEvent onSwitchActive;
    public UnityEvent onSwitchInactive;

    [Header("Self-retrieved References")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        spriteRenderer.color = inactive;
        isActive = false;
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

        if (stayingColliders.Count == 0)
        {
            onSwitchActive.Invoke();
            spriteRenderer.color = active;
            isActive = true;
            animator.SetBool("isActive", isActive);
        }
        stayingColliders.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out var _)
            && !other.TryGetComponent<BatController>(out var _)
            && !other.TryGetComponent<ProjectileController>(out var _)
        )
        {
            return;
        }
        
        stayingColliders.Remove(other);
        if (stayingColliders.Count == 0)
        {
            onSwitchInactive.Invoke();
            spriteRenderer.color = inactive;
            isActive = false;
            animator.SetBool("isActive", isActive);
        }
    }
}
