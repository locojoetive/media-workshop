using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(SpriteResolver))]
[RequireComponent(typeof(SpriteRenderer))]
public class HUDHitPointIconHide : MonoBehaviour
{
    public GameObject hitPointIcon;
    public SpriteRenderer spriteRenderer;
    public SpriteResolver spriteResolver;
    public Image image;

    private void Awake()
    {
        spriteResolver = GetComponent<SpriteResolver>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
    }

    public void HideIcon()
    {
        hitPointIcon.SetActive(false);
    }

    public void ShowIcon()
    {
        hitPointIcon.SetActive(true);
    }

    private void LateUpdate()
    {
        image.sprite = spriteRenderer.sprite;
        hitPointIcon.GetComponent<Image>().sprite = spriteRenderer.sprite;
    }
}
