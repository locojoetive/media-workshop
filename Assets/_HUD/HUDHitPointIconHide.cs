using UnityEngine;

public class HUDHitPointIconHide : MonoBehaviour
{
    public GameObject hitPointIcon;

    public void HideIcon()
    {
        Debug.Log("Hiding hit point icon");
        hitPointIcon.SetActive(false);
    }

    public void ShowIcon()
    {
        hitPointIcon.SetActive(true);
    }
}
