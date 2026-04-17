using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioResolver : MonoBehaviour
{
    private AudioSource _audioSource;
    private SoundLibrary _soundLibrary;
    public string entryName;

    [Header("Debug")]
    public string objectId;
    public string id;

    public void Init()
    {
        if (transform.parent != null)
        {
            objectId = transform.parent.gameObject.GetInstanceID().ToString();
        }
        else
        {
            objectId = gameObject.GetInstanceID().ToString();
        }
        id = objectId + "_" + entryName;
        _audioSource = GetComponent<AudioSource>();
        _soundLibrary = GameManager.Instance.SoundManager.soundLibrary;
        if (string.IsNullOrEmpty(entryName))
        {
            Debug.LogError("Entry name is not set!", this);
            return;
        }
        var clip = _soundLibrary.GetClipByEntryName(entryName);
        if (clip == null)
        {
            Debug.LogError($"No AudioClip found for entry name: {entryName}", this);
            return;
        }
        _audioSource.clip = clip;
    }

    internal void PlayClip(bool loop)
    {
        _audioSource.loop = loop;
        _audioSource.Play();
    }

    internal bool IsPlaying()
    {
        return _audioSource.isPlaying;
    }

    internal void StopClip()
    {
        _audioSource.Stop();
    }

    internal void PlayAudioClipByEntryNameWithRandomPitch(float minPitch, float maxPitch)
    {
        _audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        PlayClip(false);
    }

    internal void SetPlaybackSpeed(float speedFactor)
    {
        _audioSource.pitch = speedFactor;
    }
}
