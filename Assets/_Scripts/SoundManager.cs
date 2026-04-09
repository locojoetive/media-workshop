using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public SoundLibrary soundLibrary;

    public Dictionary<string, AudioResolver> audioResolversDictionary;

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Rebuild the dictionary of audio resolvers when a new scene is loaded
        audioResolversDictionary = new Dictionary<string, AudioResolver>();
        foreach (var resolver in FindObjectsByType<AudioResolver>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            audioResolversDictionary[resolver.entryName] = resolver;
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
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

    internal void SetPlaybackSpeed(string playerStepsEntryName, float speedFactor)
    {
        if (audioResolversDictionary.TryGetValue(playerStepsEntryName, out var resolver))
        {
            resolver.SetPlaybackSpeed(speedFactor);
            return;
        }
        Debug.LogWarning($"No AudioResolver found for entry name: {playerStepsEntryName}");
    }
}
