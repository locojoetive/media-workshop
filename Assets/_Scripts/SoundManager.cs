using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public SoundLibrary soundLibrary;

    public Dictionary<string, AudioResolver> audioResolversDictionary;

    public void RegisterAudioResolver(AudioResolver resolver)
    {
        if (audioResolversDictionary == null)
        {
            audioResolversDictionary = new Dictionary<string, AudioResolver>();
        }
        if (!audioResolversDictionary.ContainsKey(resolver.objectId))
        {
            audioResolversDictionary.Add(resolver.id, resolver);
        }
        else
        {
            Debug.LogWarning($"AudioResolver with objectId {resolver.objectId} is already registered.");
        }
    }

    private void Start()
    {
        var gameManagerInstanceId = transform.parent.gameObject.GetInstanceID().ToString();
        PlayAudioClipByEntryName(gameManagerInstanceId, "music", true);
    }

    public void StopAudioClipByEntryName(string objectId, string entryName)
    {
        var resolverId = objectId + "_" + entryName;
        if (audioResolversDictionary.TryGetValue(resolverId, out var resolver))
        {
            resolver.StopClip();
            return;
        }
        Debug.LogWarning($"No AudioResolver found for entry name: {resolverId}");
    }

    public void PlayAudioClipByEntryName(string objectId, string entryName, bool loop = false)
    {
        var resolverId = objectId + "_" + entryName;
        if (audioResolversDictionary.TryGetValue(resolverId, out var resolver))
        {
            resolver.PlayClip(loop);
            return;
        }
        Debug.LogWarning($"No AudioResolver found for entry name: {resolverId}");
    }

    public bool IsClipPlaying(string objectId, string entryName)
    {
        var resolverId = objectId + "_" + entryName;
        if (audioResolversDictionary.TryGetValue(resolverId, out var resolver))
        {
            return resolver.IsPlaying();
        }
        Debug.LogWarning($"No AudioResolver found for entry name: {resolverId}");
        return false;
    }

    internal void PlayAudioClipByEntryNameWithRandomPitch(string objectId, string entryName, float minPitch, float maxPitch)
    {
        var resolverId = objectId + "_" + entryName;
        if (audioResolversDictionary.TryGetValue(resolverId, out var resolver))
        {
            resolver.PlayAudioClipByEntryNameWithRandomPitch(minPitch, maxPitch);
            return;
        }
        Debug.LogWarning($"No AudioResolver found for entry name: {resolverId}");
    }
}
