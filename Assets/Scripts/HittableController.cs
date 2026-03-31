using System;
using UnityEngine;

public class HittableController : MonoBehaviour
{
    public int health = 3;
    public Action onTakeDamage;
    public Action onDeath;

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            onDeath?.Invoke();
            Destroy(gameObject);
            return;
        }
        onTakeDamage?.Invoke();
    }
}
