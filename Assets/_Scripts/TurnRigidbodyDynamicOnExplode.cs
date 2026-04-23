using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TurnRigidbodyDynamicOnExplode : MonoBehaviour
{
    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Explode(Vector2 explosionForce)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(explosionForce, ForceMode2D.Impulse);

        // Randomize the spin so every object reacts differently
        float rotationIntensity = 5f; 
        float randomSpin = Random.Range(-rotationIntensity, rotationIntensity);
        
        rb.AddTorque(randomSpin, ForceMode2D.Impulse);
    }
}
