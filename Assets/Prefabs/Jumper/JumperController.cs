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
    public Collider2D col;
    public SpriteRenderer spriteRenderer;
    public MovementInfluenceController movementInfluenceController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        movementInfluenceController = GetComponent<MovementInfluenceController>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);
        isWallAhead = Physics2D.OverlapCircle(wallCheckAhead.position, groundDistance, groundMask);
        
        if (isWallAhead)
        {
            FlipX();
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
        }

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
        HandleAnimation();
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
    

    #region Animation
    [Header("Animation Settings")]
    public float maxVelocity = 20f;
    public float minimumScale = 0.8f;
    public float maximumScale = 1.2f;
    private void HandleAnimation()
    {
        if (movementInfluenceController.isStunned)
        {
            return;
        }
        // horizontal
        var velocityFactorX = Mathf.Abs(rb.linearVelocityX);
        var horizontalMovementFactorY = MathHelper.Map(Mathf.Abs(velocityFactorX), 0f, maxVelocity, 1f, minimumScale);
        var horizontalMovementFactorX = 1f + 1f - horizontalMovementFactorY;

        // vertical
        var velocityFactorY = Mathf.Abs(rb.linearVelocity.y);
        var verticalMovementFactorY = MathHelper.Map(velocityFactorY, 0f, maxVelocity, 1f, maximumScale);
        var verticalMovementFactorX = 1f + 1f - verticalMovementFactorY;

        spriteRenderer.transform.localScale = new Vector3(
            verticalMovementFactorX * horizontalMovementFactorX,
            verticalMovementFactorY * horizontalMovementFactorY,
            1f
        );
    }
    #endregion Animation
}
