using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RendererController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetAlpha(float alpha)
    {
        var color = spriteRenderer.material.color;
        color.a = alpha;
        spriteRenderer.material.color = color;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void SetOverlayColor(Color color)
    {
        spriteRenderer.material.SetColor("_OverlayColor", color);
    }

    public void SetFillAmount(float fillAmount)
    {
        spriteRenderer.material.SetFloat("_FillAmount", fillAmount);
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    public void SetScale(float scaleX, float scaleY)
    {
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}
