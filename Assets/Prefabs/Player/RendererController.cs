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
    internal void Flash(float duration)
    {
        StartCoroutine(FlashRedCoroutine(duration));
    }

    private IEnumerator FlashRedCoroutine(float duration)
    {
        float elapsedTime = 0f;
        spriteRenderer.material.SetColor("_OverlayColor", Color.white);
        while (elapsedTime < duration)
        {
            spriteRenderer.material.SetFloat("_FillAmount", 1f);
            yield return new WaitForSeconds(flashInterval);
            elapsedTime += flashInterval;
            spriteRenderer.material.SetFloat("_FillAmount", 0f);
            yield return new WaitForSeconds(flashInterval);
            elapsedTime += flashInterval;
        }
        spriteRenderer.color = Color.white;
    }

    internal void SetAlpha(float alpha)
    {
        var color = spriteRenderer.material.color;
        color.a = alpha;
        spriteRenderer.material.color = color;
    }
}
