using System;
using Unity.VisualScripting;
using UnityEngine;

public enum FlyStateType
{
    Flying,
    CatchingFall,
    Fluttering,
}

public enum PatrolStateType
{
    Moving,
    Staying,
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MovementInfluenceController))]
public class FlutterController : MonoBehaviour
{
    
    [Header("References")]
    public Transform wallCheckAhead;

    [Header("Wall Detection Settings")]
    public float wallDistance = 0.2f;
    public LayerMask wallMask;
    public bool isWallAhead;

    [Header("Debug")]
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public MovementInfluenceController movementInfluenceController;

    #region LifeCycle
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementInfluenceController = GetComponent<MovementInfluenceController>();
    }


    private void Start()
    {
        originalHeight = transform.position.y;
        attack.onIdle = OnIdle;
        attack.onCoolDown = OnCoolDown;
        attack.onAnticipation = OnAnticipation;
        attack.onAttack = OnAttack;
        attack.onRecovery = OnRecovery;
    }


    private void FixedUpdate()
    {
        isWallAhead = Physics2D.OverlapCircle(wallCheckAhead.position, wallDistance, wallMask);
        var isPlayerDetected = playerDetectorController.isPlayerDetected;

        // horizontal movement
        HandleHorizontalMovement(isPlayerDetected);

        // vertical movement
        HandleFlyState(isPlayerDetected);
    }

    private void HandleHorizontalMovement(bool isPlayerDetected)
    {
        var movementInfluence = movementInfluenceController.movementInfluence;
        if (!isPlayerDetected)
        {
            var moveSpeed = speed * Mathf.Sign(transform.localScale.x);
            var horizontalVelocity = patrolState == PatrolStateType.Moving && !isWallAhead
                ? moveSpeed * movementInfluence + rb.linearVelocity.x * (1f - movementInfluence)
                : 0f;
            rb.linearVelocity = new Vector2(horizontalVelocity, rb.linearVelocity.y);
        }
        else
        {
            var moveDirection = playerDetectorController.target.position + Vector3.up * heightAbovePlayer - transform.position;
            var moveSpeed = speed * Mathf.Sign(moveDirection.x) * Mathf.Clamp01(Mathf.Abs(moveDirection.x));
            var horizontalVelocity = moveSpeed * movementInfluence + rb.linearVelocity.x * (1f - movementInfluence);
            rb.linearVelocity = new Vector2(horizontalVelocity, rb.linearVelocity.y);
            Debug.DrawRay(transform.position, moveDirection, Color.blue);
            HandleAttack();
        }

    }

    private void Update()
    {
        var isPlayerDetected = playerDetectorController.isPlayerDetected;
        if (isPlayerDetected)
        {
            Debug.DrawLine(
                playerDetectorController.target.position + heightAbovePlayer * Vector3.up,
                playerDetectorController.target.position + heightAbovePlayer * Vector3.up + 10f * Vector3.right,
                Color.green
            );
            return;
        }
        Debug.DrawLine(new Vector2(-10, originalHeight), new Vector2(10, originalHeight), Color.red);
        HandlePatrolMovement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(collision.GetContact(0).normal);
        }
    }
    #endregion LifeCycle

    #region Patrol
    [Header("Patrol Settings")]
    public PatrolStateType patrolState;
    public float stayDuration = 0.5f;
    public float stayTime = 0f;
    public float moveDuration = 3f;
    public float moveTime = 0f;

    private void HandlePatrolMovement()
    {
        switch (patrolState)
        {
            case PatrolStateType.Moving:
                if (moveTime < moveDuration)
                {
                    moveTime += Time.deltaTime;
                    return;
                }
                patrolState = PatrolStateType.Staying;
                stayTime = 0f;
                break;
            case PatrolStateType.Staying:
                if (stayTime < stayDuration)
                {
                    stayTime += Time.deltaTime;
                    return;
                }
                patrolState = PatrolStateType.Moving;
                FlipX();
                moveTime = 0f;
                break;
        }
    }

    #endregion Patrol


    #region Horizontal Movement
    

    [Header("Movement Settings")]
    public float speed = 5f;
    private void FlipX()
    {
        transform.localScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z
        );
    }
    #endregion Horizontal Movement

    #region Flying
    [Header("Fly Settings")]
    public FlyStateType flyState;
    float originalHeight = 0f;
    public float idleCatchFallForce = 10f;
    public float idleFlutterForce = 10f;
    public float idleMaximumFlutterSpeed = 10f;
    private void HandleFlyState(bool isPlayerDetected)
    {
        var targetHeight = isPlayerDetected
            ? playerDetectorController.target.position.y + heightAbovePlayer
            : originalHeight;
        var targetCatchFallForce = isPlayerDetected
            ? attackCatchFallForce
            : idleCatchFallForce;
        var targetFlutterForce = isPlayerDetected
            ? attackFlutterForce
            : idleFlutterForce;
        var targetMaximumFlutterSpeed = isPlayerDetected
            ? attackMaximumFlutterSpeed
            : idleMaximumFlutterSpeed;
        switch (flyState)
        {
            case FlyStateType.Flying:
                if (rb.position.y < targetHeight)
                {
                    flyState = FlyStateType.CatchingFall;
                }
                break;
            case FlyStateType.CatchingFall:
                if (rb.linearVelocityY < 0f)
                {
                    rb.AddForce(targetCatchFallForce * Vector2.up, ForceMode2D.Force);
                }
                var maxCatchFallSpeed = Mathf.Min(rb.linearVelocityY, 0f);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxCatchFallSpeed);
                if (rb.linearVelocityY >= 0f)
                {
                    flyState = FlyStateType.Fluttering;
                }
                break;
            case FlyStateType.Fluttering:
                if (rb.linearVelocityY < targetMaximumFlutterSpeed)
                {
                    rb.AddForce(targetFlutterForce * Vector2.up, ForceMode2D.Force);
                }
                var maxFlutterSpeed = Mathf.Min(rb.linearVelocityY, targetMaximumFlutterSpeed);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFlutterSpeed);
                if (rb.position.y > targetHeight || rb.linearVelocityY >= targetMaximumFlutterSpeed)
                {
                    flyState = FlyStateType.Flying;
                }
                break;
        }
    }
    
    #endregion Flying

    #region Attack
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 5f;
    public PlayerDetectorController playerDetectorController;
    public float heightAbovePlayer = 3f;
    public AttackState attack;
    public float attackCatchFallForce = 10f;
    public float attackFlutterForce = 10f;
    public float attackMaximumFlutterSpeed = 10f;
    public float attackRange = 1f;
    
    private void HandleAttack()
    {
        if (!playerDetectorController.isPlayerDetected)
        {
            // end attack if player is lost
            if (attack.state != AttackStateType.Idle)
            {
                attack.EnterAttackState();
                spriteRenderer.color = Color.white;
            }
            return;
        }
        // initialize attack when player is detected for the first time
        else if (attack.state == AttackStateType.Idle)
        {
            attack.EnterAttackState();
            spriteRenderer.color = Color.blue;
        }
        
        var distanceOnXAxis = Mathf.Abs(playerDetectorController.target.position.x - transform.position.x);
        attack.canAttack = distanceOnXAxis < attackRange;
        attack.Update(Time.deltaTime);
    }

    private void OnCoolDown()
    {
        spriteRenderer.color = Color.blue;
    }

    private void OnAnticipation()
    {
        spriteRenderer.color = Color.yellow;
    }

    private void OnAttack()
    {
        spriteRenderer.color = Color.red;
        var projectile = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            projectileSpawnPoint.rotation
        ).GetComponent<FlutterProjectileController>();
        projectile.Initialize(projectileSpeed);
    }

    private void OnRecovery()
    {
        spriteRenderer.color = Color.green;
    }

    private void OnIdle()
    {
        spriteRenderer.color = Color.white;
    }
    #endregion Attack

}
