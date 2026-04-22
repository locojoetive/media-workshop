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

    public void Explode(Vector2 explisionForce)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        var position = rb.position;
        rb.AddForce(explisionForce, ForceMode2D.Impulse);
    }
}
