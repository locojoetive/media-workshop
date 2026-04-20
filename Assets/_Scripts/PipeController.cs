using UnityEngine;

public class PipeController : MonoBehaviour
{
    [Header("References")]
    public GameObject prefabToSpawn;
    public Transform spawnPosition;
    public PipeTriggerZoneController pipeTriggerZoneController;

    [Header("Debug")]
    public GameObject spawnedObject;
    public bool isActive;
    private void Awake()
    {
        pipeTriggerZoneController = GetComponentInChildren<PipeTriggerZoneController>();
    }

    private void OnEnable()
    {
        pipeTriggerZoneController.onPlayerEnterPipeTriggerZone += () => isActive = true;
        pipeTriggerZoneController.onPlayerExitPipeTriggerZone += () => isActive = false;
    }

    private void OnDisable()
    {
        pipeTriggerZoneController.onPlayerEnterPipeTriggerZone -= () => isActive = true;
        pipeTriggerZoneController.onPlayerExitPipeTriggerZone -= () => isActive = false;
    }


    void Update()
    {
        if (!isActive || (spawnedObject != null && spawnedObject.activeInHierarchy))
        {
            return;
        }

        spawnedObject = Instantiate(prefabToSpawn, spawnPosition.position, spawnPosition.rotation);
        if (spawnedObject.TryGetComponent<Rigidbody2D>(out var rigidbody2D))
        {
            rigidbody2D.linearVelocity = transform.up * 10f;
        }
    }
}
