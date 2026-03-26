using System;
using System.Collections;
using UnityEngine;

public class BatController : MonoBehaviour
{
    public bool isHitting;
    public bool isAiming;
    public float force;
    public float coolDownEndTime;
    public float coolDownDuration;

    public Quaternion originalRotation;
    public Quaternion aimBaseRotation;
    public Color originalColor;


    public Collider2D col;
    public SpriteRenderer spriteRenderer;


    private void Awake()
    {
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Start()
    {
        originalRotation = transform.rotation;
        aimBaseRotation = Quaternion.Euler(0, 0, 90f);
        FadeOut();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isHitting)
        {
            return;
        }
        if (collision.gameObject.TryGetComponent<MovementInfluenceController>(out var movementInfluence))
        {
            movementInfluence.FadeMovementForBounceDuration(1f, force);
        }
        if (collision.gameObject.TryGetComponent<Rigidbody2D>(out var rigidbody))
        {
            var factor = (coolDownEndTime - Time.time) / coolDownDuration;
            var hitForce = Mathf.Lerp(force, force / 2f, AnimationHelper.EaseInQubic(factor));
            rigidbody.AddForce(transform.up * hitForce, ForceMode2D.Impulse);
        }
    }

    public void Rotate(Vector2 direction)
    {
        transform.rotation = aimBaseRotation * Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }


    public void Swing()
    {
        if (isHitting)
        {
            return;
        }
        isAiming = false;
        isHitting = true;
        coolDownEndTime = Time.time + coolDownDuration;
        StartCoroutine(SwingCoroutine());
    }
    private IEnumerator SwingCoroutine()
    {
        yield return new WaitForSeconds(coolDownDuration);
        isHitting = false;
        transform.rotation = originalRotation;
        FadeOut();
    }

    private void FadeOut()
    {
        Color color = originalColor;
        color.a = 0.8f;
        spriteRenderer.color = color;
        col.enabled = false;
    }

    public void StartAim()
    {
        isAiming = true;
        spriteRenderer.color = originalColor;
        col.enabled = true;
    }
}
