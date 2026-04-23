using UnityEngine;
using UnityEngine.EventSystems;

public class MobileSwingBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private PlayerInputController PlayerInputController => GameManager.Instance.PlayerInputController;
    public Vector2 position;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        PlayerInputController.OnMobileMouseLeftButton(true);
        PlayerInputController.OnMobileMousePosition(eventData.position);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        PlayerInputController.OnMobileMouseLeftButton(false);
        PlayerInputController.OnMobileMousePosition(position);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        PlayerInputController.OnMobileMousePosition(eventData.position);
    }
}
