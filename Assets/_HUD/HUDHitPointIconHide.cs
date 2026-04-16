using UnityEngine;

public class HUDHitPointIconHide : MonoBehaviour
{
    public GameObject hitPointIcon;

    public void HideIcon()
    {
        hitPointIcon.SetActive(false);
    }

    public void ShowIcon()
    {
        hitPointIcon.SetActive(true);
    }
}
