using System.Collections.Generic;
using UnityEngine;

public class HUDHitPoints : MonoBehaviour
{
    private PlayerController playerController;
    public GameObject hitPointPrefab;
    public List<HUDHitPointIconHide> hitPointIcons = new List<HUDHitPointIconHide>();

    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        var hitPoints = playerController.hitPoints;
        for (int i = 0; i < hitPoints; i++)
        {
            hitPointIcons.Add(Instantiate(hitPointPrefab, transform).GetComponent<HUDHitPointIconHide>());
        }

        playerController.onTakeDamage += TakeDamage;
    }

    private void TakeDamage(int remainingHitPoints)
    {
        if (remainingHitPoints < 0)
        {
            return;
        }
        hitPointIcons[remainingHitPoints].HideIcon();
    }
}
