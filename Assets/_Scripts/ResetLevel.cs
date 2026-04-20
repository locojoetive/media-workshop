using UnityEngine;

public class ResetLevel : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out var _))
        {
            GameManager.Instance.CheckPointManager.SetCheckPoint(0);
            GameManager.Instance.LoadSceneManager.ReloadCurrentScene();
        }
    }
}
