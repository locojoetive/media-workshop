using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadSceneManager : MonoBehaviour
{
    PlayerInputController playerInput => GameManager.Instance.PlayerInputController;
    public Image backdrop;
    public bool isLoadingScene = false;

    private void Start()
    {
        backdrop = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if (playerInput.selectPressed)
        {
            ReloadCurrentScene();
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
        isLoadingScene = false;
        StartCoroutine(FadeInCoroutine(1f));
    }

    public void LoadScene(int sceneIndex)
    {
        if (isLoadingScene)
        {
            return;
        }
        isLoadingScene = true;
        StartCoroutine(FadeOutAndLoadSceneCoroutine(1f, sceneIndex));
    }

    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator FadeOutAndLoadSceneCoroutine(float duration, int sceneIndex)
    {
        Debug.Log("Starting Fade Out");
        float elapsedTime = 0f;
        Color originalColor = backdrop.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            backdrop.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        backdrop.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float elapsedTime = 0f;
        Color originalColor = backdrop.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            backdrop.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        backdrop.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}
