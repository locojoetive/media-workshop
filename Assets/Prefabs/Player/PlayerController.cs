using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerActionType
{
    Move,
    Jump,
    Attack,
    Interact
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementInfluenceController))]
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
    public Rigidbody2D _rigidbody;
    public RendererController _renderer;
    public MovementInfluenceController movementInfluenceController;

    [Header("Debugging")]
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
        _rigidbody = GetComponent<Rigidbody2D>();
        movementInfluenceController = GetComponent<MovementInfluenceController>();

        _renderer = GetComponentInChildren<RendererController>();
    }

    private void Update()
    {
        if (isDead || movementInfluenceController.isStunned)
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
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer) != null;
        isOnWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer) != null;
    }

    private void HandleMovement()
    {
        var currentMoveSpeed = playerInput.leftStickDirection.x * moveSpeed;
        var currentSprintSpeed = playerInput.leftStickDirection.x * playerInput.rightTriggerValue * (sprintSpeed - moveSpeed);
        var totalMoveSpeed = currentMoveSpeed + Mathf.Sign(currentMoveSpeed) * currentSprintSpeed;
        var movementInfluence = movementInfluenceController.movementInfluence;
        var horizontalVelocity = totalMoveSpeed * movementInfluence + _rigidbody.linearVelocity.x * (1f - movementInfluence);

        _rigidbody.linearVelocity = new Vector2(horizontalVelocity, _rigidbody.linearVelocity.y);
        if (currentMoveSpeed > 0)
        {
            _renderer.FlipX(false);
        }
        else if (currentMoveSpeed < 0)
        {
            _renderer.FlipX(true);
        }
    }
    
    private void HandleJump()
    {
        var inputJump = playerInput.buttonSouthPressed;
        jumpSpeedLow = Mathf.Abs(_rigidbody.linearVelocity.y) <= 0.5f;
        if (isJumping && jumpSpeedLow)
        {
            isJumping = false;
        }
        else if (!isJumping && isGrounded && inputJump && !wasInputJumpPressedInLastFrame)
        {
            var force = jumpForce;
            _rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            isJumping = true;
        }    
        wasInputJumpPressedInLastFrame = inputJump;
    }

    private IEnumerator StunAndInvincibleCoroutine()
    {
        movementInfluenceController.Stun();
        isInvincible = true;

        yield return new WaitForSeconds(movementInfluenceController.stunDuration);
        movementInfluenceController.Unstun();

        yield return new WaitForSeconds(invincibilityDuration - stunDuration);
        isInvincible = false;
    }

    public void TakeDamage(Vector2 collisionNormal)
    {
        if (isInvincible || isDead)
        {
            return;
        }
        hitPoints--;
        onTakeDamage?.Invoke(hitPoints);
        StartCoroutine(StunAndInvincibleCoroutine());

        // Animate
        _renderer.FlashRed(invincibilityDuration);

        // Knockback
        Debug.Log($"Applying knockback with normal {collisionNormal.normalized}");
        var knockbackDirection = -collisionNormal.normalized - _renderer.facingDirection * Vector2.right;
        _rigidbody.linearVelocity = knockbackDirection.normalized * knockbackForce;

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