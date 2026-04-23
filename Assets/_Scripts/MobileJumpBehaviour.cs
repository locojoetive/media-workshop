using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MobileJumpBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private PlayerInputController PlayerInputController => GameManager.Instance.PlayerInputController;
    private Image jumpImage;

    private void Awake()
    {
        jumpImage = GetComponent<Image>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerInputController.OnMobileSouthButton(true);
        var color = Color.white;
        color.a = 0.75f;
        jumpImage.color = color;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerInputController.OnMobileSouthButton(false);
        var color = Color.white;
        color.a = 0.25f;
        jumpImage.color = color;
    }
}