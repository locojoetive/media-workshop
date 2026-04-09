using UnityEngine;

public class FitToRendererSize : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private BoxCollider2D _col;

    private void Start()
    {
        _col = GetComponent<BoxCollider2D>();
    }

    public void Update()
    {
        _col.transform.position = spriteRenderer.transform.position;
        _col.size = spriteRenderer.size;
    }
}
