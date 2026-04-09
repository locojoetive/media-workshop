using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public SoundLibrary soundLibrary;

    public Dictionary<string, AudioResolver> audioResolversDictionary;

    private void Awake()
    {

        audioResolversDictionary = new Dictionary<string, AudioResolver>();
        foreach (var resolver in FindObjectsByType<AudioResolver>(FindObjectsSortMode.None))
        {
            audioResolversDictionary[resolver.entryName] = resolver;
        }
    }

    private void Start()
    {
        PlayAudioClipByEntryName("music", true);
    }

    public void StopAudioClipByEntryName(string entryName)
    {
        if (audioResolversDictionary.TryGetValue(entryName, out var resolver))
        {
            resolver.StopClip();
            return;
        }
        Debug.LogWarning($"No AudioResolver found for entry name: {entryName}");
    }

    public void PlayAudioClipByEntryName(string entryName, bool loop = false)
    {
        if (audioResolversDictionary.TryGetValue(entryName, out var resolver))
        {
            resolver.PlayClip(loop);
            return;
        }
        Debug.LogWarning($"No AudioResolver found for entry name: {entryName}");
    }

    public bool IsClipPlaying(string entryName)
    {
        if (audioResolversDictionary.TryGetValue(entryName, out var resolver))
        {
            return resolver.IsPlaying();
        }
        Debug.LogWarning($"No AudioResolver found for entry name: {entryName}");
        return false;
    }

    internal void PlayAudioClipByEntryNameWithRandomPitch(string entryName, float minPitch, float maxPitch)
    {
        if (audioResolversDictionary.TryGetValue(entryName, out var resolver))
        {
            resolver.PlayAudioClipByEntryNameWithRandomPitch(minPitch, maxPitch);
            return;
        }
        Debug.LogWarning($"No AudioResolver found for entry name: {entryName}");
    }
}
