using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public enum PlayerActionType
{
    Move,
    Jump,
    Attack,
    Interact
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(RigidbodyController))]
public class PlayerController : MonoBehaviour
{
    PlayerInputController playerInput => GameManager.Instance.PlayerInputController;

    public PlayerActionType[] playerActions;

    public int hitPoints = 3;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    public float knockbackForce = 10f;
    public float stunDuration = 0.5f;
    public float invincibilityDuration = 1f;

    [Header("Ground Check Settings")]
    public LayerMask groundLayer;

    public Transform groundCheck;
    public Transform wallCheck;
    public float checkRadius = 0.2f;

    [Header("Self-Retrieved References")]
    public RendererController _renderer;
    public RigidbodyController rigidbodyController;
    public ParticleSystem particleSystem;


    [Header("Debugging")]
    public int damagePerHit = 1;
    public bool isGrounded;
    public bool isJumping;
    public bool jumpSpeedLow;
    public bool isOnWall;
    public bool isInvincible;
    public bool isDead;
    private bool wasInputJumpPressedInLastFrame = false;
    public Action<int> onTakeDamage;

    private void Awake()
    {
        rigidbodyController = GetComponent<RigidbodyController>();

        _renderer = GetComponentInChildren<RendererController>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if (isDead || rigidbodyController.isStunned)
        {
            return;
        }

        if (PlayerCan(PlayerActionType.Move))
        {
            HandleMovement();
        }
        if (PlayerCan(PlayerActionType.Jump))
        {
            HandleJump();
        }
        if (PlayerCan(PlayerActionType.Attack))
        {
            HandleAttack();
        }

        HandleAnimation();
    }

    private void HandleAnimation()
    {
        if (!rigidbodyController.isStunned)
        {
            var inputHorizontalMovement = playerInput.leftStickDirection.x;
            var inputSprint = playerInput.rightTriggerValue;

            // horizontal
            var inputFactor = (Mathf.Abs(inputHorizontalMovement) + inputSprint) * 0.5f;
            var horizontalMovementFactorY = MathHelper.Map(Mathf.Abs(inputFactor), 0f, 1f, 1f, 0.8f);
            var horizontalMovementFactorX = 1f + 1f - horizontalMovementFactorY;

            // vertical
            var velocityFactor = Mathf.Abs(rigidbodyController.LinearVelocityY);
            var verticalMovementFactorY = MathHelper.Map(velocityFactor, 0f, 20f, 1f, 1.2f);
            var verticalMovementFactorX = 1f + 1f - verticalMovementFactorY;

            _renderer.transform.localScale = new Vector3(
                verticalMovementFactorX * horizontalMovementFactorX,
                verticalMovementFactorY * horizontalMovementFactorY,
                1f
            );
        }


        if (isGrounded && !particleSystem.isPlaying)
        {
            particleSystem.Play();
        }
        else if (!isGrounded && particleSystem.isPlaying)
        {
            particleSystem.Stop();
        } 
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer) != null;
        isOnWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer) != null;
    }

    private void HandleMovement()
    {
        var inputHorizontalMovement = playerInput.leftStickDirection.x;
        var inputSprint = playerInput.rightTriggerValue;
        var currentMoveSpeed = inputHorizontalMovement * moveSpeed;
        if (currentMoveSpeed > 0)
        {
            FlipX(false);
        }
        else if (currentMoveSpeed < 0)
        {
            FlipX(true);
        }
        
        if (isOnWall)
        {
            return;
        }
        
        var currentSprintSpeed = inputHorizontalMovement
            * inputSprint
            * (sprintSpeed - moveSpeed);
        var totalMoveSpeed = currentMoveSpeed + currentSprintSpeed;
        var movementInfluence = rigidbodyController.movementInfluence;
        var horizontalVelocity = totalMoveSpeed * movementInfluence + rigidbodyController.LinearVelocityX * (1f - movementInfluence);

        rigidbodyController.SetVelocityX(horizontalVelocity);
    }

    private void FlipX(bool flip)
    {
        transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);
    }
    
    private void HandleJump()
    {
        var inputJump = playerInput.buttonSouthPressed;
        jumpSpeedLow = Mathf.Abs(rigidbodyController.LinearVelocityY) <= 0.5f;
        if (isJumping && jumpSpeedLow)
        {
            isJumping = false;
        }
        else if (!isJumping && isGrounded && inputJump && !wasInputJumpPressedInLastFrame)
        {
            var force = jumpForce;
            rigidbodyController.SetVelocityY(force);
            isJumping = true;
        }    
        wasInputJumpPressedInLastFrame = inputJump;
    }

    [Header("Attack")]
    public BatController batController;
    public bool wasAttackingInLastFrame;
    public Vector2 lastAttackDirection;
    public float minimumSnapVelocity = 100f;

    private void HandleAttack()
    {
        if (batController.isSwinging)
        {
            return;
        }
        var attackDirection = playerInput.rightStickDirection;
        var isAttacking = attackDirection != Vector2.zero;

        if (isAttacking && !wasAttackingInLastFrame)
        {
            batController.StartAim();
        }
        if (isAttacking && wasAttackingInLastFrame)
        {
            batController.SetRotationFromDirection(-lastAttackDirection);
        }
        else if (wasAttackingInLastFrame)
        {
            Debug.Log("HIT");
            batController.Swing(lastAttackDirection);
        }
        
        if (isAttacking)
        {
            lastAttackDirection = attackDirection;
        }
        wasAttackingInLastFrame = isAttacking;
        
    }

    private IEnumerator StunAndInvincibleCoroutine()
    {
        rigidbodyController.Stun();
        isInvincible = true;

        _renderer.Flash(stunDuration);
        var elapsedTime = 0f;
        while (elapsedTime < stunDuration)
        {
            var factor = 2f * Mathf.PI * (elapsedTime / stunDuration);
            var scale = 0.75f - 0.25f * Mathf.Cos(factor);
            _renderer.transform.localScale = new Vector3(scale, scale, 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rigidbodyController.Unstun();

        _renderer.SetAlpha(0.5f);
        yield return new WaitForSeconds(invincibilityDuration - stunDuration);
        _renderer.SetAlpha(1f);
        isInvincible = false;
    }

    public void TakeDamage(Vector2 collisionNormal)
    {
        if (isInvincible || isDead)
        {
            return;
        }
        hitPoints -= damagePerHit;
        onTakeDamage?.Invoke(hitPoints);
        StartCoroutine(StunAndInvincibleCoroutine());

        // Knockback
        Debug.Log($"Applying knockback with normal {collisionNormal.normalized}");
        var knockbackDirection = -collisionNormal.normalized - Mathf.Sign(transform.localScale.x) * Vector2.right;
        rigidbodyController.SetVelocityX(knockbackDirection.normalized.x * knockbackForce);
        rigidbodyController.SetVelocityY(knockbackDirection.normalized.y * knockbackForce);

        // Kill
        if (hitPoints <= 0)
        {
            isDead = true;
            GameManager.Instance.LoadSceneManager.ReloadCurrentScene();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
        if (wallCheck != null)
        {
            Gizmos.color = isOnWall ? Color.green : Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }
    }

    private bool PlayerCan(PlayerActionType actionType)
    {
        return playerActions.Contains(actionType);
    }
}