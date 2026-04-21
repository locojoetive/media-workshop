using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class ToggleSwitchController : MonoBehaviour
{
    public Color active;
    public Color inactive;
    public bool isActive = false;
    public UnityEvent onSwitchActive;
    public UnityEvent onSwitchInactive;

    [Header("Self-retrieved References")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public string audioResolverId;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        isActive = false;
        spriteRenderer.color = inactive;
    }

    private void Start()
    {
        audioResolverId = GetComponentInChildren<AudioResolver>().objectId;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out var _)
            && !other.TryGetComponent<BatController>(out var _)
            && !other.TryGetComponent<ProjectileController>(out var _)
            && !other.TryGetComponent<HittableController>(out var _)
        )
        {
            return;
        }




        if (isActive)
        {
            return;
        }
        onSwitchActive.Invoke();
        spriteRenderer.color = active;
        isActive = true;
        animator.SetBool("isActive", isActive);
        GameManager.Instance.SoundManager.PlayAudioClipByEntryName(audioResolverId, "switch_activate");
    }
}
