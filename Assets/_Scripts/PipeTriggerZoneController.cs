using System;
using UnityEngine;

public class PipeTriggerZoneController : MonoBehaviour
{
    public Action onPlayerEnterPipeTriggerZone;
    public Action onPlayerExitPipeTriggerZone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out var _))
        {
            return;
        }
        onPlayerEnterPipeTriggerZone?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out var _))
        {
            return;
        }
        onPlayerExitPipeTriggerZone?.Invoke();
    }
}
