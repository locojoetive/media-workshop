using System;
using System.Collections;
using UnityEngine;

public class TrampolineController : MonoBehaviour
{   
    [Header("Bounce Settings")]
    public float bounceForce = 10f;
    public float pressDuration = 0.5f;
    public float releaseDuration = 0.1f;
    public Vector2 maxScaleFactor = new Vector2(1.2f, 1.2f);
    public Vector2 minScaleFactor = new Vector2(0.8f, 0.5f);

    [Header("Debugging")]
    public Vector2 originalScale;
    public Vector2 originalPosition;
    public bool isBouncing;
    public bool isResetting;
    public Coroutine bounceCoroutine;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var playerController = collider.gameObject.GetComponent<PlayerController>();
        if (playerController != null && !isBouncing && !isResetting)
        {
            isBouncing = true;
            bounceCoroutine = StartCoroutine(Bounce(playerController));
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        var playerController = collider.gameObject.GetComponent<PlayerController>();
        if (playerController != null && isBouncing)
        {
            isResetting = true;
            isBouncing = false;
            StopCoroutine(bounceCoroutine);
            StartCoroutine(ResetTrampoline(playerController));
        }
    }

    private IEnumerator ResetTrampoline(PlayerController playerController)
    {
        var maxScaleX = maxScaleFactor.x * originalScale.x;
        var maxScaleY = maxScaleFactor.y * originalScale.y;
        var minScaleX = minScaleFactor.x * originalScale.x;
        var minScaleY = minScaleFactor.y * originalScale.y;

        float requiredReleaseDuration = releaseDuration * (1f - (transform.localScale.y - minScaleY) / (maxScaleY - minScaleY));
        var halfReleaseDuration = requiredReleaseDuration / 2f;
        var elapsedTime = 0f;
        
        // Expand up phase
        while (elapsedTime < halfReleaseDuration)
        {
            float factor = elapsedTime / halfReleaseDuration;

            float scaleY = Mathf.Lerp(minScaleY, maxScaleY, factor);
            float scaleX = Mathf.Lerp(originalScale.x, minScaleX, factor);
            transform.localScale = new Vector3(scaleX, scaleY, 1f);

            float positionOffsetY = (originalScale.y - scaleY) / 2f;
            originalPosition.x = transform.position.x;
            transform.position = originalPosition - new Vector2(0f, positionOffsetY);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Return to normal phase
        elapsedTime = 0f;
        while (elapsedTime < halfReleaseDuration)
        {
            float factor = elapsedTime / halfReleaseDuration;

            float scaleY = Mathf.Lerp(originalScale.y * maxScaleY, originalScale.y, factor);
            float scaleX = Mathf.Lerp(minScaleX, originalScale.x, factor);
            transform.localScale = new Vector3(scaleX, scaleY, 1f);

            float positionOffsetY = (originalScale.y - scaleY) / 2f;
            originalPosition.x = transform.position.x;
            transform.position = originalPosition - new Vector2(0f, positionOffsetY);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = new Vector3(originalScale.x, originalScale.y, 1f);
        transform.position = originalPosition;
        
        playerController.RetrieveBounce();
        isBouncing = false;
        isResetting = false;
    }

    private IEnumerator Bounce(PlayerController playerController)
    {
        var maxScaleX = maxScaleFactor.x * originalScale.x;
        var minScaleY = minScaleFactor.y * originalScale.y;
        originalPosition = transform.position;

        float elapsedTime = 0f;
        // Compress phase
        while (elapsedTime < pressDuration)
        {
            float factor = elapsedTime / pressDuration;

            float scaleY = Mathf.Lerp(originalScale.y, minScaleY, factor);
            float scaleX = Mathf.Lerp(originalScale.x, maxScaleX, factor);
            transform.localScale = new Vector3(scaleX, scaleY, 1f);

            float positionOffsetY = (originalScale.y - scaleY) / 2f;
            originalPosition.x = transform.position.x;
            transform.position = originalPosition - new Vector2(0f, positionOffsetY);

            float bounce = bounceForce * EaseInQubic(factor);
            playerController.GiveBounce(bounce);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private static float EaseInQubic(float t)
    {
        return t * t * t;
    }
}
