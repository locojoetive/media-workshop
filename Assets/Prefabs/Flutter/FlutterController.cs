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
    public SpriteRenderer spriteRenderer;
    public MovementInfluenceController rigidbodyController;

    #region LifeCycle
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidbodyController = GetComponent<MovementInfluenceController>();
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

        // horizontal movement
        HandleHorizontalMovement();

        // vertical movement
        HandleFlyState();
    }

    private void Update()
    {
        HandlePatrolMovement();
        HandleAnimation();
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

    #region Patrol Time
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
                FlipX(-transform.localScale.x);
                moveTime = 0f;
                break;
        }
    }

    #endregion Patrol Time

    #region Horizontal Movement

    [Header("Movement Settings")]
    public float speed = 5f;

    private void FlipX(float direction)
    {
        transform.localScale = new Vector3(
            Mathf.Sign(direction) * Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private void HandleHorizontalMovement()
    {
        var isPlayerDetected = playerDetectorController.isPlayerDetected;
        if (!isPlayerDetected)
        {
            var moveSpeed = speed * Mathf.Sign(transform.localScale.x);
            var horizontalVelocity = patrolState == PatrolStateType.Moving && !isWallAhead
                ? moveSpeed
                : 0f;
            rigidbodyController.SetVelocityX(horizontalVelocity);
        }
        else
        {
            var moveDirection = playerDetectorController.target.position + Vector3.up * heightAbovePlayer - transform.position;
            if (Mathf.Sign(moveDirection.x) != Mathf.Sign(transform.localScale.x))
            {
                FlipX(Mathf.Sign(moveDirection.x));
            }
            var moveSpeed = speed * Mathf.Sign(moveDirection.x) * Mathf.Clamp01(Mathf.Abs(moveDirection.x));
            var horizontalVelocity = moveSpeed;
            rigidbodyController.SetVelocityX(horizontalVelocity);
            Debug.DrawRay(transform.position, moveDirection, Color.blue);
            HandleAttack();
        }
    }
    #endregion Horizontal Movement
    
    #region Vertical Movement
    [Header("Fly Settings")]
    public FlyStateType flyState;
    float originalHeight = 0f;
    public float idleCatchFallForce = 10f;
    public float attackCatchFallForce = 10f;
    public float idleFlutterForce = 10f;
    public float attackFlutterForce = 10f;
    public float idleMaximumFlutterForce = 10f;
    public float attackMaximumFlutterForce = 10f;

    private void HandleFlyState()
    {
        var isPlayerDetected = playerDetectorController.isPlayerDetected;
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
            ? attackMaximumFlutterForce
            : idleMaximumFlutterForce;
        switch (flyState)
        {
            case FlyStateType.Flying:
                if (rigidbodyController.Position.y < targetHeight)
                {
                    flyState = FlyStateType.CatchingFall;
                }
                break;
            case FlyStateType.CatchingFall:
                var catchFallforce = rigidbodyController.LinearVelocityY + targetCatchFallForce;
                var maxCatchFallSpeed = Mathf.Min(catchFallforce, 0f);
                rigidbodyController.SetVelocityY(maxCatchFallSpeed);
                if (rigidbodyController.LinearVelocityY >= 0f)
                {
                    flyState = FlyStateType.Fluttering;
                }
                break;
            case FlyStateType.Fluttering:
                var flutterforce = rigidbodyController.LinearVelocityY + targetFlutterForce;
                var maxFlutterSpeed = Mathf.Min(flutterforce, targetMaximumFlutterSpeed);
                rigidbodyController.SetVelocityY(maxFlutterSpeed);
                if (rigidbodyController.Position.y > targetHeight || rigidbodyController.LinearVelocityY >= targetMaximumFlutterSpeed)
                {
                    flyState = FlyStateType.Flying;
                }
                break;
        }
    }

    #endregion Vertical Movement

    #region Attack
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 5f;
    public float recoilForce = 10f;
    public PlayerDetectorController playerDetectorController;
    public float heightAbovePlayer = 3f;
    public AttackState attack;
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
        rigidbodyController.SetVelocity(new Vector2(0.75f * rigidbodyController.LinearVelocityX, recoilForce));
        rigidbodyController.FadeMovementForDuration(1f);
        spriteRenderer.color = Color.red;
        var projectile = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            projectileSpawnPoint.rotation
        ).GetComponent<ProjectileController>();
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
        var horizontalMovementFactorY = MathHelper.Map(Mathf.Abs(velocityFactorX), 0f, maxVelocity, 1f, minimumScale);
        var horizontalMovementFactorX = 1f + 1f - horizontalMovementFactorY;

        // vertical
        var velocityFactorY = Mathf.Abs(rigidbodyController.LinearVelocityY);
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
