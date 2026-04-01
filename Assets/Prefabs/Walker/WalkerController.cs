using System;
using UnityEngine;

[RequireComponent(typeof(MovementInfluenceController))]
[RequireComponent(typeof(Rigidbody2D))]
public class WalkerController : MonoBehaviour
{
    [Header("References")]
    public Transform groundCheck;
    public Transform groundCheckAhead;
    public Transform wallCheckAhead;
    public MovementInfluenceController movementInfluenceController;

    [Header("Settings")]
    public float speed = 5f;
    public float stayDuration = 0.5f;
    public float stayTime = 0f;
    
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Debug")]
    public bool isGrounded;
    public bool isWallAhead;
    public bool isGroundAhead;
    public bool isStaying;
    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movementInfluenceController = GetComponent<MovementInfluenceController>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);
        isGroundAhead = Physics2D.OverlapCircle(groundCheckAhead.position, groundDistance, groundMask);
        isWallAhead = Physics2D.OverlapCircle(wallCheckAhead.position, groundDistance, groundMask);

        if (isStaying || (!isGrounded && isWallAhead))
        {
            return;
        }
        var moveSpeed = speed * Mathf.Sign(transform.localScale.x);
        var movementInfluence = movementInfluenceController.movementInfluence;
        var horizontalVelocity = moveSpeed * movementInfluence + rb.linearVelocity.x * (1f - movementInfluence);
        rb.linearVelocity = new Vector2(horizontalVelocity, rb.linearVelocity.y);
    }

    private void Update()
    {   
        if (isGrounded && (!isGroundAhead || isWallAhead))
        {
            if (!isStaying)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                isStaying = true;
                stayTime = 0f;
            }
            else if (isStaying && stayTime < stayDuration)
            {
                stayTime += Time.deltaTime;
            }
            else
            {
                FlipX();
                isStaying = false;
            }
        }
        else
        {
            isStaying = false;
        }
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        if (movementInfluenceController.isStunned)
        {
            return;
        }
        // horizontal
        var velocityFactorX = Mathf.Abs(rb.linearVelocityX);
        var horizontalMovementFactorY = MathHelper.Map(Mathf.Abs(velocityFactorX), 0f, 20f, 1f, 0.8f);
        var horizontalMovementFactorX = 1f + 1f - horizontalMovementFactorY;

        // vertical
        var velocityFactorY = Mathf.Abs(rb.linearVelocity.y);
        var verticalMovementFactorY = MathHelper.Map(velocityFactorY, 0f, 20f, 1f, 1.2f);
        var verticalMovementFactorX = 1f + 1f - verticalMovementFactorY;

        transform.localScale = new Vector3(
            verticalMovementFactorX * horizontalMovementFactorX,
            verticalMovementFactorY * horizontalMovementFactorY,
            1f
        );
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
        Gizmos.color = isGroundAhead ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheckAhead.position, groundDistance);
        Gizmos.color = isWallAhead ? Color.green : Color.red;
        Gizmos.DrawWireSphere(wallCheckAhead.position, groundDistance);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(collision.GetContact(0).normal);
        }
    }
}
