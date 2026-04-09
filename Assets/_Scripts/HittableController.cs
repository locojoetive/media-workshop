using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RigidbodyController))]
public class HittableController : MonoBehaviour
{
    [Header("Settings")]
    public int health = 3;
    public float knockbackForce = 5f;
    public float invincibilityDuration = 1f;
    public float stunDuration = 0.5f;
    public float flashInterval = 0.166f;

    [Header("Self-Retrieved References")]
    public RigidbodyController rigidbodyController;
    private RendererController rendererController;

    [Header("Debug")]
    public bool isInvincible = false;
    public bool isDead = false;

    public Action onTakeDamage;
    public Action onDeath;

    private void Awake()
    {
        rigidbodyController = GetComponent<RigidbodyController>();
        rendererController = GetComponentInChildren<RendererController>();
    }

    public void TakeDamage(Vector2 collisionNormal)
    {
        if (isInvincible || isDead)
        {
            return;
        }
        isInvincible = true;
        health--;

        ShakeCamera.Instance.ShakeForDuration(0.5f, stunDuration, transform.position);
        StartCoroutine(StunAndInvincibleCoroutine());
        onTakeDamage?.Invoke();

        if (health <= 0)
        {
            isDead = true;
            rigidbodyController.FreeFromConstraints();
            onDeath?.Invoke();
        }
        
        // Knockback
        var knockbackDirection = -collisionNormal.normalized - Mathf.Sign(transform.localScale.x) * Vector2.right;
        rigidbodyController.SetVelocityX(knockbackDirection.normalized.x * knockbackForce);
        rigidbodyController.SetVelocityY(knockbackDirection.normalized.y * knockbackForce);
    }

    private IEnumerator StunAndInvincibleCoroutine()
    {   
        float elapsedTime = 0f;
        float flashTime = 0f;
        bool flashOn = false;
        rigidbodyController.Stun();
        rendererController.SetOverlayColor(Color.white);
        while (elapsedTime < stunDuration)
        {
            var factor = 2f * Mathf.PI * (elapsedTime / stunDuration);
            var scale = 0.75f - 0.25f * Mathf.Cos(factor);
            rendererController.SetScale(scale);
            elapsedTime += Time.deltaTime;

            flashTime += Time.deltaTime;
            if (flashTime >= flashInterval)
            {
                rendererController.SetFillAmount(flashOn ? 1f : 0f);
                flashOn = !flashOn;
                flashTime = 0f;
            }
            yield return null;
        }
        rendererController.SetFillAmount(0f);
        rendererController.SetColor(Color.white);
        rigidbodyController.Unstun();
        rendererController.SetScale(1f);

        rendererController.SetAlpha(0.5f);
        yield return new WaitForSeconds(invincibilityDuration - stunDuration);
        rendererController.SetAlpha(1f);
        isInvincible = false;
    }
}
