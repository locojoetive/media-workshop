using System;
using UnityEngine;

public enum AttackStateType
{
    Idle,
    CoolDown,
    Anticipation,
    Attack,
    Recovery
}

[RequireComponent(typeof(MovementInfluenceController))]
public class WalkerSpitterController : MonoBehaviour
{
    [Header("References")]
    public Transform groundCheck;
    public Transform groundCheckAhead;
    public Transform wallCheckAhead;
    public PlayerDetectorController playerDetectorController;

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
    public SpriteRenderer spriteRenderer;
    public MovementInfluenceController movementInfluenceController;
    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movementInfluenceController = GetComponent<MovementInfluenceController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);
        isGroundAhead = Physics2D.OverlapCircle(groundCheckAhead.position, groundDistance, groundMask);
        isWallAhead = Physics2D.OverlapCircle(wallCheckAhead.position, groundDistance, groundMask);

        if (playerDetectorController.isPlayerDetected)
        {
            return;
        }

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
        
        HandleAttack();
        if (attackState != AttackStateType.Idle)
        {
            return;
        }

        if (!isGrounded || isGroundAhead && !isWallAhead)
        {
            isStaying = false;
            return;
        }

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
    
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 5f;
    public AttackStateType attackState;
    public float coolDownDuration = 1f;
    public float coolDownTime = 0f;
    public float anticipationDuration = 0.5f;
    public float anticipationTime = 0f;
    public float attackDuration = 1f;
    public float attackTime = 0f;
    public float recoveryDuration = 0.5f;
    public float recoveryTime = 0f;
    private void HandleAttack()
    {
        if (!playerDetectorController.isPlayerDetected)
        {
            // end attack if player is lost
            if (attackState != AttackStateType.Idle)
            {
                attackState = AttackStateType.Idle;
                coolDownTime = 0f;
                anticipationTime = 0f;
                attackTime = 0f;
                recoveryTime = 0f;
                spriteRenderer.color = Color.white;
            }
            return;
        }
        // initialize attack when player is detected for the first time
        else if (attackState == AttackStateType.Idle)
        {
            attackState = AttackStateType.CoolDown;
            spriteRenderer.color = Color.blue;
        }

        // move spawn point
        var spawnPointDistance = Vector2.Distance(projectileSpawnPoint.position, transform.position);
        var playerDirection = Vector3.ClampMagnitude(playerDetectorController.target.position - transform.position, spawnPointDistance);
        if (playerDirection.x < 0 && transform.localScale.x > 0
            || playerDirection.x > 0 && transform.localScale.x < 0)
        {
            FlipX();
        }
        projectileSpawnPoint.position = transform.position + playerDirection;

        switch (attackState)
        {
            case AttackStateType.CoolDown:
                if (coolDownTime < coolDownDuration)
                {
                    coolDownTime += Time.deltaTime;
                    return;
                }
                spriteRenderer.color = Color.yellow;
                attackState = AttackStateType.Anticipation;
                coolDownTime = 0f;
                break;
            case AttackStateType.Anticipation:
                if (anticipationTime < anticipationDuration)
                {
                    anticipationTime += Time.deltaTime;
                    return;
                }
                spriteRenderer.color = Color.red;
                attackState = AttackStateType.Attack;
                anticipationTime = 0f;
                break;
            case AttackStateType.Attack:
                if (attackTime < attackDuration)
                {
                    attackTime += Time.deltaTime;
                    return;
                }
                var projectile = Instantiate(
                    projectilePrefab,
                    projectileSpawnPoint.position,
                    projectileSpawnPoint.rotation
                ).GetComponent<WalkerSpitterProjectileController>();
                projectile.Initialize(
                    playerDetectorController.target.position,
                    projectileSpeed
                );
                spriteRenderer.color = Color.green;
                attackState = AttackStateType.Recovery;
                attackTime = 0f;
                break;
            case AttackStateType.Recovery:
                if (recoveryTime < recoveryDuration)
                {
                    recoveryTime += Time.deltaTime;
                    return;
                }
                spriteRenderer.color = Color.blue;
                attackState = AttackStateType.CoolDown;
                recoveryTime = 0f;
                break;
            default:
                break;
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
        Gizmos.color = isGroundAhead ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheckAhead.position, groundDistance);
        Gizmos.color = isWallAhead ? Color.green : Color.red;
        Gizmos.DrawWireSphere(wallCheckAhead.position, groundDistance);
    }
}
