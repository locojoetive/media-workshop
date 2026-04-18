using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TurnRigidbodyDynamicOnExplode : MonoBehaviour
{
    public Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Explode(Vector2 explisionForce)
    {
        Debug.Log("BOOM");
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        var position = rigidbody.position;
        rigidbody.AddForce(explisionForce, ForceMode2D.Impulse);
    }
}
