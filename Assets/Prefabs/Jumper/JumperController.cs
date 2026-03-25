using UnityEngine;

public enum MoveStateType
{
    IDLE,
    JUMP
}

public class JumperController : MonoBehaviour
{
    [Header("References")]
    public Transform groundCheck;
    public Transform wallCheckAhead;

    [Header("Settings")]
    public float jumpHeight = 5f;
    public float jumpLength = 0.5f;
    public float idleDuration = 0.5f;
    public float idleTime = 0f;
    
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Debug")]
    public bool isGrounded;
    public bool isWallAhead;
    public Rigidbody2D rb;
    public MovementInfluenceController movementInfluenceController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movementInfluenceController = GetComponent<MovementInfluenceController>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);
        isWallAhead = Physics2D.OverlapCircle(wallCheckAhead.position, groundDistance, groundMask);
        if (!isGrounded)
        {
            return;
        }
        
        if (idleTime < idleDuration)
        {
            idleTime += Time.deltaTime;
            return;
        }

        idleTime = 0f;
        var horizontalJumpDirection = Mathf.Sign(transform.localScale.x) * jumpLength;
        rb.linearVelocity = new Vector2(horizontalJumpDirection, jumpHeight);
    }

    private void Update()
    {
        if (!isGrounded && isWallAhead)
        {
            FlipX();
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
        }
    }

    private void FlipX()
    {
        transform.localScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
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
