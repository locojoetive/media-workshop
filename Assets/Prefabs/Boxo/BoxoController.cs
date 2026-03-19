using UnityEngine;

public class BoxoController : MonoBehaviour
{
    [Header("References")]
    public Transform groundCheck;
    public Transform groundCheckAhead;
    public Transform wallCheckAhead;

    [Header("Settings")]
    public float speed = 5f;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Debug")]
    public bool isGrounded;
    public bool isWallAhead;
    public bool isGroundAhead;
    public Rigidbody2D rb;
    public RendererController _renderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponentInChildren<RendererController>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);
        isGroundAhead = Physics2D.OverlapCircle(groundCheckAhead.position, groundDistance, groundMask);
        isWallAhead = Physics2D.OverlapCircle(wallCheckAhead.position, groundDistance, groundMask);
    }

    private void Update()
    {
        if (isGrounded)
        {
            var moveSpeed = speed * Mathf.Sign(_renderer.transform.localScale.x);
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }

        if (isGrounded && (!isGroundAhead || isWallAhead))
        {
            _renderer.FlipX();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        Gizmos.color = isGroundAhead ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheckAhead.position, groundDistance);
        Gizmos.color = isWallAhead ? Color.green : Color.red;
        Gizmos.DrawWireSphere(wallCheckAhead.position, groundDistance);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(collision.GetContact(0).normal);
        }
    }
}
