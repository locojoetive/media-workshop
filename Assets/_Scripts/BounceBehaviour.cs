using UnityEngine;

public class BounceBehaviour : MonoBehaviour
{
    public string audioResolverId;
    private void Start()
    {
        audioResolverId = GetComponentInChildren<AudioResolver>().objectId;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.Instance.SoundManager.PlayAudioClipByEntryNameWithRandomPitch(audioResolverId, "projectile_bounce", 0.9f, 1.1f);
    }
}
