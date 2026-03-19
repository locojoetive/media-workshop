using System;
using System.Collections;
using UnityEngine;

public class RendererController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public float flashInterval = 0.166f;
    internal void FlashRed(float duration)
    {
        StartCoroutine(FlashRedCoroutine(duration));
    }

    private IEnumerator FlashRedCoroutine(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashInterval);
            elapsedTime += flashInterval;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashInterval);
            elapsedTime += flashInterval;
        }
        spriteRenderer.color = Color.white;
    }

    internal void FlipX(bool flip)
    {
        transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);
    }

    internal void FlipX()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }
}
