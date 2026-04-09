using System;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class ShakeCamera : MonoBehaviour
{
    public static ShakeCamera Instance { get; private set; }
    
    public CinemachineImpulseSource source;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        source = GetComponent<CinemachineImpulseSource>();
    }

    // Basic shake
    public void Shake(float intensity)
    {
        source.GenerateImpulse(new Vector3(intensity, intensity, 0f));
    }

    // Basic shake
    public void ShakeForDuration(float intensity, float duration)
    {
        source.ImpulseDefinition.ImpulseDuration = duration;
        source.GenerateImpulse(new Vector3(intensity, intensity, 0f));
    }

    // Directional shake (e.g., hit from the left)
    public void Shake(float intensity, Vector3 direction)
    {
        source.GenerateImpulse(direction.normalized * intensity);
    }

    // Full control shake with X, Y, Z intensity
    public void Shake(float intensityX, float intensityY)
    {
        source.GenerateImpulse(new Vector3(intensityX, intensityY, 0f));
    }

    internal void ShakeForDuration(float intensity, float stunDuration, Vector3 sourcePosition)
    {
        source.ImpulseDefinition.ImpulseDuration = stunDuration;
        source.GenerateImpulseAt(sourcePosition, new Vector3(intensity, intensity, 0f));
    }
}
