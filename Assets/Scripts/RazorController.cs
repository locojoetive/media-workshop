using UnityEngine;

public class RazorController : MonoBehaviour
{
    [Header("References")]
    public Transform groundCheck;
    public Transform groundCheckAhead;
    public Transform wallCheckAhead;
    public Transform rendererTransform;

    [Header("Settings")]
    public float speed = 5f;
    public float rotateSpeed = 360f;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Debug")]
    public bool isGrounded;
    public bool isWallAhead;
    public bool isGroundAhead;
    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);
        isGroundAhead = Physics2D.OverlapCircle(groundCheckAhead.position, groundDistance, groundMask);
        isWallAhead = Physics2D.OverlapCircle(wallCheckAhead.position, groundDistance, groundMask);
    }

    private void Update()
    {
        var moveSpeed = speed * Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

        rendererTransform.RotateAround(
            rendererTransform.position,
            Vector3.forward,
            -Mathf.Sign(transform.localScale.x) * rotateSpeed * Time.deltaTime
        );

        if (isGrounded && (!isGroundAhead || isWallAhead))
        {
            transform.localScale = new Vector3(
                -transform.localScale.x,
                transform.localScale.y,
                transform.localScale.z
            );
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<HittableController>(out var hittableController))
        {
            hittableController.TakeDamage(collision.GetContact(0).normal);
        }
    }
}
