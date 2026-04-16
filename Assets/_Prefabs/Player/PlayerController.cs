using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(RigidbodyController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HittableController))]
public class PlayerController : MonoBehaviour
{
    PlayerInputController playerInput => GameManager.Instance.PlayerInputController;
    private SoundManager SoundManager => GameManager.Instance.SoundManager;

    public enum PlayerActionType { Move, Jump, Attack, Interact }
    public PlayerActionType[] playerActions;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;

    [Header("Ground Check Settings")]
    public LayerMask groundLayer;

    public Transform groundCheck;
    public Transform wallCheck;
    public float checkRadius = 0.2f;

    [Header("Self-Retrieved References")]
    public RendererController _renderer;
    public RigidbodyController rigidbodyController;
    public Animator animator;
    public ParticleSystem particles;
    public HittableController hittableController;


    [Header("Debugging")]
    public bool isGrounded;
    public bool isJumping;
    public bool jumpSpeedLow;
    public bool isOnWall;
    private bool wasInputJumpPressedInLastFrame = false;

    private void Awake()
    {
        rigidbodyController = GetComponent<RigidbodyController>();
        _renderer = GetComponentInChildren<RendererController>();
        particles = GetComponentInChildren<ParticleSystem>();
        animator = GetComponent<Animator>();
        hittableController = GetComponent<HittableController>();
        hittableController.onTakeDamage += () =>
        {
            SoundManager.PlayAudioClipByEntryNameWithRandomPitch("player_damage", 0.8f, 1.2f);
        };
        hittableController.onDeath += Die;
    }

    private void Update()
    {
        if (hittableController.isDead || rigidbodyController.isStunned)
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

    private const string PlayerStepsEntryName = "player_steps";
    private const string PlayerJumpEntryName = "player_jump";

    private void HandleAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(rigidbodyController.LinearVelocityX));

        if (!rigidbodyController.isStunned)
        {
            var inputHorizontalMovement = playerInput.leftStickDirection.x;
            var inputSprint = playerInput.rightTriggerValue;

            // horizontal
            var inputFactor = (Mathf.Abs(inputHorizontalMovement) + inputSprint) * 0.5f;
            var horizontalMovementFactorY = MathHelper.ClampAndMap(Mathf.Abs(inputFactor), 0f, 1f, 1f, 0.8f);
            var horizontalMovementFactorX = 1f + 1f - horizontalMovementFactorY;

            // vertical
            var velocityFactor = Mathf.Abs(rigidbodyController.LinearVelocityY);
            var verticalMovementFactorY = MathHelper.ClampAndMap(velocityFactor, 0f, 20f, 1f, 1.2f);
            var verticalMovementFactorX = 1f + 1f - verticalMovementFactorY;

            _renderer.transform.localScale = new Vector3(
                verticalMovementFactorX * horizontalMovementFactorX,
                verticalMovementFactorY * horizontalMovementFactorY,
                1f
            );
        }


        if (isGrounded && !particles.isPlaying)
        {
            particles.Play();
        }
        else if (!isGrounded && particles.isPlaying)
        {
            particles.Stop();
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
        rigidbodyController.SetVelocityX(totalMoveSpeed);

        if (!Mathf.Approximately(totalMoveSpeed, 0f) && !SoundManager.IsClipPlaying(PlayerStepsEntryName))
        {
            SoundManager.PlayAudioClipByEntryName(PlayerStepsEntryName);
        }
        animator.SetFloat("Speed", Mathf.Abs(totalMoveSpeed));
    }

    private void FlipX(bool flip)
    {
        transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);
    }
    
    [Header("Jump Tuning")]
    public float jumpHeight = 4f;       // Total units to rise
    public float timeToReachApex = 0.4f; // How fast he elevates
    public float hangTime = 0.1f;       // How long he stays at peak
    public float fallDuration = 0.3f;   // How fast he falls back down

    public float jumpTimer;
    public enum JumpState { Grounded, Rising, Hanging, Falling }
    public JumpState currentJumpState = JumpState.Grounded;

    private void HandleJump()
    {
        bool inputJumpPressed = playerInput.buttonSouthPressed;

        switch (currentJumpState)
        {
            case JumpState.Grounded:
                if (isGrounded && inputJumpPressed && !wasInputJumpPressedInLastFrame)
                {
                    jumpTimer = 0;
                    currentJumpState = JumpState.Rising;
                    SoundManager.PlayAudioClipByEntryNameWithRandomPitch(PlayerJumpEntryName, 0.8f, 1.2f);
                }
                break;

            case JumpState.Rising:
                jumpTimer += Time.deltaTime;
                float risingProgress = jumpTimer / timeToReachApex;

                if (!inputJumpPressed)
                {
                    currentJumpState = JumpState.Falling;
                    break;
                }
                if (risingProgress >= 1.0f)
                {
                    jumpTimer = 0;
                    currentJumpState = JumpState.Hanging;
                    break;
                }

                float ease = AnimationHelper.EaseOutSquare(risingProgress);
                float upVelocity = jumpHeight * ease;
                rigidbodyController.SetVelocityY(upVelocity);
                break;

            case JumpState.Hanging:
                jumpTimer += Time.deltaTime;
                if (!inputJumpPressed || jumpTimer >= hangTime)
                {
                    jumpTimer = 0;
                    currentJumpState = JumpState.Falling;
                    break;
                }
                // Maintain slight hover or zero Y velocity here
                rigidbodyController.SetVelocityY(0);
                break;

            case JumpState.Falling:
                if (isGrounded) 
                {
                    currentJumpState = JumpState.Grounded;
                }
                break;
        }

        wasInputJumpPressedInLastFrame = inputJumpPressed;
    }

    [Header("Attack")]
    public BatController batController;
    public bool wasAttackingInLastFrame;
    public Vector2 lastAttackDirection;
    public float minimumSnapVelocity = 100f;

    public bool isAttackingWithMouse = false;
    public bool wasAttackingWithMouseInLastFrame = false;
    public Vector2 originalMousePosition;
    public Vector2 lastMouseAimDirection;
    public float minimumMouseSnapVelocity = 100f;

    private void HandleAttack()
    {
        if (batController.isSwinging)
        {
            return;
        }
        

        // Either use mouse inputs
        var isAttackingWithMouse = playerInput.mouseLeftButtonPressed;
        var latestMousePosition = playerInput.mousePosition;
        if (isAttackingWithMouse && !wasAttackingWithMouseInLastFrame)
        {
            originalMousePosition = latestMousePosition;
            batController.StartAimMouse(originalMousePosition);
        }
        else if (isAttackingWithMouse && wasAttackingWithMouseInLastFrame)
        {
            batController.SetRotationFromDirectionForMouse(latestMousePosition, originalMousePosition);
        }
        else if (wasAttackingWithMouseInLastFrame)
        {
            batController.SwingForMouse(-lastMouseAimDirection);
        }
        wasAttackingWithMouseInLastFrame = isAttackingWithMouse;
        
        // Or use gamepad inputs
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
            batController.SwingForController(lastAttackDirection);
        }
        if (isAttacking)
        {
            lastAttackDirection = attackDirection;
        }
        wasAttackingInLastFrame = isAttacking;
    }

    public void Die()
    {
        GameManager.Instance.LoadSceneManager.ReloadCurrentScene();
        GameManager.Instance.SoundManager.PlayAudioClipByEntryName("player_dead");
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