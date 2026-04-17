using UnityEngine;

public enum MoveStateType
{
    IDLE,
    JUMP
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RigidbodyController))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(HittableController))]
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
    public Collider2D col;
    public RendererController rendererController;
    public RigidbodyController rigidbodyController;
    public HittableController hittableController;
    public Animator animator;

    public string audioResolverId;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        rigidbodyController = GetComponent<RigidbodyController>();
        animator = GetComponent<Animator>();
        hittableController = GetComponent<HittableController>();
        rendererController = GetComponentInChildren<RendererController>();
    }

    private void Start()
    {

        audioResolverId = GetComponentInChildren<AudioResolver>().objectId;
        hittableController.onTakeDamage += () =>
        {
            GameManager.Instance.SoundManager.PlayAudioClipByEntryNameWithRandomPitch(audioResolverId, "jumper_damage", 0.8f, 1.2f);
        };
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);
        isWallAhead = Physics2D.OverlapCircle(wallCheckAhead.position, groundDistance, groundMask);
        
        if (isWallAhead)
        {
            FlipX();
            rigidbodyController.SetVelocityX(-rigidbodyController.LinearVelocityX);
            return;
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
        rigidbodyController.SetVelocity(new Vector2(horizontalJumpDirection, jumpHeight));
        GameManager.Instance.SoundManager.PlayAudioClipByEntryNameWithRandomPitch(audioResolverId, "jumper_jump", 0.8f, 1.2f);
    }

    private void Update()
    {
        HandleAnimation();
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rigidbodyController.LinearVelocityY);
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
        if (hittableController.isDead)
        {
            return;
        }
        
        if (collision.gameObject.TryGetComponent<HittableController>(out var otherHittableController))
        {
            otherHittableController.TakeDamage(collision.GetContact(0).normal);
        }
    }
    

    #region Animation
    [Header("Animation Settings")]
    public float maxVelocity = 20f;
    public float minimumScale = 0.8f;
    public float maximumScale = 1.2f;
    private void HandleAnimation()
    {
        if (rigidbodyController.isStunned)
        {
            return;
        }
        // horizontal
        var velocityFactorX = Mathf.Abs(rigidbodyController.LinearVelocityX);
        var horizontalMovementFactorOnScaleY = MathHelper.ClampAndMap(Mathf.Abs(velocityFactorX), 0f, maxVelocity, 1f, minimumScale);
        var horizontalMovementFactorOnScaleX = 1f + 1f - horizontalMovementFactorOnScaleY;

        // vertical
        var velocityFactorY = Mathf.Abs(rigidbodyController.LinearVelocityY);
        var verticalMovementFactorOnScaleY = MathHelper.ClampAndMap(velocityFactorY, 0f, maxVelocity, 1f, maximumScale);
        var verticalMovementFactorOnScaleX = 1f + 1f - verticalMovementFactorOnScaleY;

        rendererController.SetScale(
            verticalMovementFactorOnScaleX * horizontalMovementFactorOnScaleX,
            verticalMovementFactorOnScaleY * horizontalMovementFactorOnScaleY
        );
    }
    #endregion Animation
}
