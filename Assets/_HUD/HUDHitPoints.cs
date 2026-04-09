using System.Collections.Generic;
using UnityEngine;

public class HUDHitPoints : MonoBehaviour
{
    private HittableController playerHittableController;
    public GameObject hitPointPrefab;
    public List<HUDHitPointIconHide> hitPointIcons;

    private void Awake()
    {
        hitPointIcons = new List<HUDHitPointIconHide>();
        playerHittableController = FindFirstObjectByType<PlayerController>().GetComponent<HittableController>();
        
        var hitPoints = playerHittableController.health;
        for (int i = 0; i < hitPoints; i++)
        {
            hitPointIcons.Add(Instantiate(hitPointPrefab, transform).GetComponent<HUDHitPointIconHide>());
        }

        playerHittableController.onTakeDamage += TakeDamage;
    }

    private void TakeDamage()
    {
        var remainingHitPoints = playerHittableController.health;
        if (0 <= remainingHitPoints && remainingHitPoints < hitPointIcons.Count)
        {
            hitPointIcons[remainingHitPoints].HideIcon();
        }
    }
}
