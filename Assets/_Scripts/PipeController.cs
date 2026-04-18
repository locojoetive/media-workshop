using UnityEngine;

public class PipeController : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform spawnPosition;
    public GameObject spawnedObject;


    void Update()
    {
        if (spawnedObject == null || !spawnedObject.activeInHierarchy)
        {
            spawnedObject = Instantiate(prefabToSpawn, spawnPosition.position, spawnPosition.rotation);
            if (spawnedObject.TryGetComponent<Rigidbody2D>(out var rigidbody2D))
            {
                rigidbody2D.linearVelocity = transform.up * 10f;
            }
        }
    }
}
