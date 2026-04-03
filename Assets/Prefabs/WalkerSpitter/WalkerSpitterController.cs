using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RigidbodyController))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
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
    public RigidbodyController rigidbodyController;
    public Animator animator;

    private void Awake()
    {
        rigidbodyController = GetComponent<RigidbodyController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        attack.onIdle = OnIdle;
        attack.onCoolDown = OnCoolDown;
        attack.onAnticipation = OnAnticipation;
        attack.onAttack = OnAttack;
        attack.onRecovery = OnRecovery;
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
            animator.SetFloat("Speed", 0f);
            return;
        }
        var moveSpeed = speed * Mathf.Sign(transform.localScale.x);
        rigidbodyController.SetVelocityX(moveSpeed);
        animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
    }

    private void Update()
    {   
        HandleAttack();
        if (attack.state != AttackStateType.Idle)
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
            rigidbodyController.SetVelocityX(0f);
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
    public float projectileSpeed = 5f;
    public AttackState attack;
    public float attackRange = 12f;

    private void HandleAttack()
    {
        if (!playerDetectorController.isPlayerDetected)
        {
            // end attack if player is lost
            if (attack.state != AttackStateType.Idle)
            {
                attack.EnterAttackState();
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
        
        var distance = Vector3.Distance(playerDetectorController.target.position, transform.position);
        attack.canAttack = distance < attackRange;
        attack.Update(Time.deltaTime);
    }

    private void OnCoolDown()
    {
    }

    private void OnAnticipation()
    {
        animator.SetBool("Attack", true);
    }

    private void OnAttack()
    {
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
    }

    private IEnumerator IgnoreCollisionsTemporarily(ProjectileController projectile)
    {
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
    }

    private void OnRecovery()
    {
        animator.SetBool("Attack", false);
    }

    private void OnIdle()
    {
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
