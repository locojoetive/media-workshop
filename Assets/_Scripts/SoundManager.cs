using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            resolver.Init();
            audioResolversDictionary[resolver.id] = resolver;
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
