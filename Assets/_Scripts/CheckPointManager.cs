using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointManager : MonoBehaviour
{
    public int currentCheckPoint;
    public Dictionary<int, CheckPointController> checkPoints;

    private void Init()
    {
        checkPoints = new Dictionary<int, CheckPointController>();
        foreach (var checkPoint in FindObjectsByType<CheckPointController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            try
            {
                checkPoints.Add(checkPoint.priority, checkPoint);
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e);
                Debug.LogError($"There probably already is a checkpoint with the same priority: {checkPoint.priority}");
            }
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Init();
        var spawnPlayerAt = checkPoints[currentCheckPoint].transform;
        var player = FindFirstObjectByType<PlayerController>();
        player.transform.position = spawnPlayerAt.position;
        player.transform.rotation = spawnPlayerAt.rotation;
    }

    public void UpdateCheckPoint(int id)
    {
        currentCheckPoint = Mathf.Max(currentCheckPoint, id);
    }
}
