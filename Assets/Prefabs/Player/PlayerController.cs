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
    public LayerMask wallLayer;
    public LayerMask hazardLayer;

    public Transform groundCheck;
    public Transform wallCheck;
    public float checkRadius = 0.2f;

    [Header("Self-Retrieved References")]
    public Rigidbody2D _rigidbody;
    public RendererController _renderer;
    public Vector2 _velocity;

    [Header("Debugging")]
    public bool _isGrounded;
    public bool _isJumping;
    public bool _jumpSpeedLow;
    public bool _isOnWall;
    public float _bounceForce;
    public bool isBouncing;
    public bool isStunned;
    public bool isInvincible;
    

    public Action<int> onTakeDamage;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _renderer = GetComponentInChildren<RendererController>();
    }

    private void Update()
    {
        if (isStunned)
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
        _isGrounded = IsGrounded();
        _isOnWall = IsOnWall();
    }

    private bool PlayerCan(PlayerActionType actionType)
    {
        return playerActions.Contains(actionType);
    }

    private void HandleMovement()
    {
        var currentMoveSpeed = playerInput.leftStickDirection.x * moveSpeed;
        var currentSprintSpeed = playerInput.rightTriggerValue * (sprintSpeed - moveSpeed);
        var totalMoveSpeed = currentMoveSpeed + Mathf.Sign(currentMoveSpeed) * currentSprintSpeed;
        _rigidbody.linearVelocity = new Vector2(totalMoveSpeed, _rigidbody.linearVelocity.y);
        if (currentMoveSpeed > 0)
        {
            _renderer.FlipX(false);
        }
        else if (currentMoveSpeed < 0)
        {
            _renderer.FlipX(true);
        }
    }
    
    private bool wasInputJumpPressedInLastFrame = false;
    private void HandleJump()
    {
        var inputJump = playerInput.buttonSouthPressed;
        _jumpSpeedLow = Mathf.Abs(_rigidbody.linearVelocity.y) <= 0.5f;
        if (_isJumping && _jumpSpeedLow)
        {
            _isJumping = false;
        }
        else if (!_isJumping && _isGrounded && inputJump && !wasInputJumpPressedInLastFrame)
        {
            var force = jumpForce;
            if (isBouncing)
            {
                force += _bounceForce;
            }
            _rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            _isJumping = true;
        }    
        wasInputJumpPressedInLastFrame = inputJump;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer) != null;
    }

    private bool IsOnWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer) != null;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isInvincible)
        {
            return;
        }

        var isHazardous = (hazardLayer & (1 << collision.gameObject.layer)) != 0;
        if (isHazardous)
        {
            TakeDamage();
            var collisionNormal = collision.GetContact(0).normal;
            Knockback(collisionNormal);
            _renderer.FlashRed(invincibilityDuration);
            if (hitPoints <= 0)
            {
                // Handle player death (e.g., respawn, game over screen)
            }
        }
    }

    private void Knockback(Vector2 collisionNormal)
    {
        isStunned = true;
        isInvincible = true;
        StartCoroutine(StunCoroutine());
        var horizontalDirection = Vector2.right * 0.2f *(_renderer.transform.localScale.x > 0 ? -1 : 1);
        var verticalDirection = Vector2.up * collisionNormal.y;
        var knockbackDirection = (horizontalDirection + verticalDirection).normalized;
        _rigidbody.linearVelocity = knockbackDirection * knockbackForce;
    }

    private IEnumerator StunCoroutine()
    {
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;

        yield return new WaitForSeconds(invincibilityDuration - stunDuration);
        isInvincible = false;
    }

    private void TakeDamage()
    {
        hitPoints--;
        onTakeDamage?.Invoke(hitPoints);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
        if (wallCheck != null)
        {
            Gizmos.color = _isOnWall ? Color.green : Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }
    }

    internal void GiveBounce(float bounceForce)
    {
        _bounceForce = bounceForce;
        isBouncing = true;
    }

    internal void RetrieveBounce()
    {
        isBouncing = false;
    }
}