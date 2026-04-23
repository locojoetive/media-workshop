using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MobileMoveBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("References")]
    public Image leftImage;
    public Image rightImage;
    
    [Header("Alpha Values")]
    public float minAlpha = 0.25f;
    public float maxAlpha = 0.75f;

    [Header("Debug")]
    public bool isPressed = false;
    public float xValue = 0f;

    private RectTransform rectTransform;
    private PlayerInputController PlayerInputController => GameManager.Instance.PlayerInputController;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        xValue = EventDataToXValue(eventData);
        PlayerInputController.OnMobileLeftStick(xValue);
        SetColors(xValue);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        xValue = EventDataToXValue(eventData);
        PlayerInputController.OnMobileLeftStick(xValue);
        SetColors(xValue);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        xValue = 0f;
        PlayerInputController.OnMobileLeftStick(0f);
        SetColors(0);
    }

    public float EventDataToXValue(PointerEventData eventData)
    {
        var position = eventData.position.x;
        var rectTransformLeftEdge = rectTransform.position.x - (rectTransform.rect.width * rectTransform.lossyScale.x) / 3f;
        var rectTransformRightEdge = rectTransform.position.x + (rectTransform.rect.width * rectTransform.lossyScale.x) / 3f;
        var mappedPosition = MathHelper.ClampAndMap(
            position,
            rectTransformLeftEdge,
            rectTransformRightEdge,
            -1f,
            1f
        );
        return mappedPosition;
    }

    public void SetColors(float xValue)
    {
        if (xValue < 0)
        {
            var left = Color.white;
            left.a = Mathf.Lerp(minAlpha, maxAlpha, -xValue);
            leftImage.color = left;

            var right = Color.white;
            right.a = minAlpha;
            rightImage.color = right;
        }
        else if (xValue > 0)
        {
            var right = Color.white;
            right.a = Mathf.Lerp(minAlpha, maxAlpha, xValue);
            rightImage.color = right;

            var left = Color.white;
            left.a = minAlpha;
            leftImage.color = left;
        }
        else
        {
            var color = Color.white;
            color.a = minAlpha;
            leftImage.color = color;
            rightImage.color = color;
        }
    }
}
