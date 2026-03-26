using UnityEngine;

public class HittableController : MonoBehaviour
{
    public int health = 3;

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
