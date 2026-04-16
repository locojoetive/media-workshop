using UnityEngine;

public class RestartFromCheckpointController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out var _))
        {
            GameManager.Instance.LoadSceneManager.ReloadCurrentScene();
        }
    }
}
