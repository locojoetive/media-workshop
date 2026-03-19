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
    public bool isDead;
    private bool wasInputJumpPressedInLastFrame = false;
    public Action<int> onTakeDamage;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _renderer = GetComponentInChildren<RendererController>();
    }

    private void Update()
    {
        if (isDead || isStunned)
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
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer) != null;
        _isOnWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer) != null;
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

    private IEnumerator StunCoroutine()
    {
        isStunned = true;
        isInvincible = true;

        yield return new WaitForSeconds(stunDuration);
        isStunned = false;

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
        StartCoroutine(StunCoroutine());

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
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
        if (wallCheck != null)
        {
            Gizmos.color = _isOnWall ? Color.green : Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }
    }

    private bool PlayerCan(PlayerActionType actionType)
    {
        return playerActions.Contains(actionType);
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