using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    public int priority;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out var _))
        {
            Debug.Log($"Player entered checkpoint {priority}");
            GameManager.Instance.CheckPointManager.UpdateCheckPoint(priority);
        }
    }
}
