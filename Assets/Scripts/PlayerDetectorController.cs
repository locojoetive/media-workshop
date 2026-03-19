using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDetectorController : MonoBehaviour
{
    public GameObject idleDetector;
    public GameObject aggressiveDetector;

    public bool isPlayerDetected;
    public bool wasPlayerDetectedInLastFrame;
    public bool isPlayerInReach;
    public Transform target;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out var playerController))
        {
            return;
        }
        isPlayerInReach = true;
        target = playerController.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out var _))
        {
            return;
        }
        isPlayerInReach = false;
        isPlayerDetected = false;
        target = null;
    }

    private void FixedUpdate()
    {
        if (!isPlayerInReach)
        {
            return;
        }
        var sourcePosition = transform.parent.position;
        Debug.DrawLine(sourcePosition, target.position, Color.red);

        RaycastHit2D hit = Physics2D.Linecast(sourcePosition, target.position, LayerMask.GetMask("Default"));
        if (hit.collider == null)
        {
            isPlayerDetected = true;
        }
        else
        {
            var visionIsBlocked = Vector2.Distance(sourcePosition, hit.point) < Vector2.Distance(sourcePosition, target.position);
            isPlayerDetected = !visionIsBlocked;
        }
    }

    private void Update()
    {
        if (wasPlayerDetectedInLastFrame && !isPlayerDetected)
        {
            idleDetector.SetActive(true);
            aggressiveDetector.SetActive(false);
        }
        else if (!wasPlayerDetectedInLastFrame && isPlayerDetected)
        {
            idleDetector.SetActive(false);
            aggressiveDetector.SetActive(true);
            wasPlayerDetectedInLastFrame = true;
        }
        wasPlayerDetectedInLastFrame = isPlayerDetected;
    }
}
