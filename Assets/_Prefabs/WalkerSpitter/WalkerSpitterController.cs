using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RigidbodyController))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HittableController))]
public class WalkerSpitterController : MonoBehaviour
{
    [Header("References")]
    public Transform groundCheck;
    public Transform groundCheckAhead;
    public Transform wallCheckAhead;
    public PlayerDetectorController playerDetectorController;

    [Header("Settings")]    
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Self-Retrieved References")]
    public SpriteRenderer spriteRenderer;
    public RigidbodyController rigidbodyController;
    public Animator animator;
    public HittableController hittableController;

    [Header("Debug")]
    public bool isGrounded;
    public bool isWallAhead;
    public bool isGroundAhead;
    public string audioResolverId;



    #region Lifecycle
    private void Awake()
    {
        rigidbodyController = GetComponent<RigidbodyController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        hittableController = GetComponent<HittableController>();
    }

    private void Start()
    {
        audioResolverId = GetComponentInChildren<AudioResolver>().objectId;
        hittableController.onTakeDamage += () =>
        {
            GameManager.Instance.SoundManager.PlayAudioClipByEntryNameWithRandomPitch(audioResolverId, "walker_spitter_damage", 0.8f, 1.2f);
        };
        hittableController.onDeath += Die;
        attack.onIdle = OnIdle;
        attack.onCoolDown = OnCoolDown;
        attack.onAnticipation = OnAnticipation;
        attack.onAttack = OnAttack;
        attack.onRecovery = OnRecovery; 
    }

    private void Die()
    { }

    private void FixedUpdate()
    {
        if (hittableController.isDead || rigidbodyController.isStunned)
        {
            return;
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);
        isGroundAhead = Physics2D.OverlapCircle(groundCheckAhead.position, groundDistance, groundMask);
        isWallAhead = Physics2D.OverlapCircle(wallCheckAhead.position, groundDistance, groundMask);
        
        HandleMovement();
    }

    private void Update()
    {
        if (hittableController.isDead || rigidbodyController.isStunned)
        {
            return;
        }
        
        animator.SetFloat("Speed", Mathf.Abs(rigidbodyController.LinearVelocityX));
        HandleAttack();
        if (attack.state != AttackStateType.Idle)
        {
            return;
        }
        HandleMovementState();
    }

    #endregion Lifecycle

    #region Movement
    [Header("Movement Settings")]
    public float speed = 5f;
    public float stayDuration = 0.5f;
    public float stayTime = 0f;
    public bool isStaying;

    private void HandleMovement()
    {
        var moveSpeed = speed * Mathf.Sign(transform.localScale.x);
        if (playerDetectorController.isPlayerDetected 
            || isStaying 
            || (!isGrounded && isWallAhead)
        )
        {
            moveSpeed = 0f;
        }
        rigidbodyController.SetVelocityX(moveSpeed);
    }

    private void HandleMovementState()
    {
        if (!isGrounded || isGroundAhead && !isWallAhead)
        {
            isStaying = false;
            return;
        }

        if (!isStaying)
        {
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
    #endregion Movement

    #region Attack
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public AttackState attack;

    private void HandleAttack()
    {
        if (!playerDetectorController.isPlayerDetected)
        {
            // end attack if player is lost
            if (attack.state != AttackStateType.Idle)
            {
                attack.ExitAttackState();
                animator.SetBool("Attack", false);
            }
            return;
        }
        // initialize attack when player is detected for the first time
        else if (attack.state == AttackStateType.Idle)
        {
            attack.EnterAttackState();
        }

        // move spawn point
        var playerDirection = playerDetectorController.target.position - transform.position;
        if (playerDirection.x < 0 && transform.localScale.x > 0
            || playerDirection.x > 0 && transform.localScale.x < 0)
        {
            FlipX();
        }
        
        attack.canAttack = true;
        attack.Update(Time.deltaTime);
    }

    private void OnRecovery() { }

    private void OnIdle() { }

    private void OnCoolDown() { }

    private void OnAnticipation()
    {
        animator.SetBool("Attack", true);
    }

    private void OnAttack()
    {
        GameManager.Instance.SoundManager.PlayAudioClipByEntryNameWithRandomPitch(audioResolverId, "walker_spitter_spit", 0.8f, 1.2f);
        var projectile = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        ).GetComponent<ProjectileController>();
        var projectileGravity = Physics2D.gravity.y * projectile.GetComponent<Rigidbody2D>().gravityScale;
        var projectileShootDirection = MathHelper.CalculateProjectileDirection(
            transform.position,
            playerDetectorController.target.position + Vector3.up,
            projectileSpeed,
            projectileGravity
        );
        StartCoroutine(IgnoreCollisionsTemporarily(projectile));
        projectile.Initialize(projectileShootDirection);
        animator.SetBool("Attack", false);
    }

    private IEnumerator IgnoreCollisionsTemporarily(ProjectileController projectile)
    {
        var projectileCollider = projectile.GetComponent<Collider2D>();
        projectileCollider.enabled = false;
        yield return new WaitForSeconds(0.2f);
        projectileCollider.enabled = true;
    }
    #endregion Attack

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
