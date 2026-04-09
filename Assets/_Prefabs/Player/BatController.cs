using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class BatController : MonoBehaviour
{
    [Header("Settings")]
    public float originalAttackForce;
    public float swingDuration;
    public float attackDuration;
    public float coolDownDuration;
    public float aimScale = 1.2f;
    public float swingScale = 2f;

    [Header("Debug")]
    public bool isSwinging;
    public bool isAiming;
    public float attackForce;
    public float swingTime;
    public float attackTime;
    public float coolDownTime;


    [Header("Self-Retrieved References")]
    public Collider2D col;
    public SpriteRenderer spriteRenderer;
    public Quaternion originalRotation;
    public Quaternion aimBaseRotation;
    public Vector3 originalScale;
    public Color originalColor;
    public ParticleSystem particles;


    private void Awake()
    {
        cinemachineBrain = FindFirstObjectByType<CinemachineBrain>();

        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        originalScale = transform.localScale;
        particles.gameObject.SetActive(false);
    }

    void Start()
    {
        originalRotation = transform.rotation;
        aimBaseRotation = Quaternion.Euler(0, 0, 90f);
        
        Color color = originalColor;
        color.a = 0.8f;
        spriteRenderer.color = color;
        col.enabled = false;
        transform.localScale = originalScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isSwinging)
        {
            return;
        }

        var trajectoryDirection = (Vector2)transform.up;
        trajectoryDirection.x = AnimationHelper.EaseInQuint(trajectoryDirection.x);
        trajectoryDirection = trajectoryDirection.normalized;

        Debug.Log($"Trajectory Direction: {trajectoryDirection}");

        if (collision.gameObject.TryGetComponent<RigidbodyController>(out var rigidbodyController))
        {
            rigidbodyController.SetVelocityInRespectToMass(trajectoryDirection * attackForce);
            rigidbodyController.FadeMovementForDuration(1f);;
            PlayEffects(attackForce, collision.GetContact(0).point);
            StartCoroutine(IgnoreCollisionsTemporarilyCoroutine(collision.otherCollider));
            
        }
        else if (collision.gameObject.TryGetComponent<Rigidbody2D>(out var rigidbody))
        {
            rigidbody.linearVelocity = trajectoryDirection * attackForce / (rigidbody.mass * rigidbody.mass);
            PlayEffects(attackForce, collision.GetContact(0).point);
            StartCoroutine(IgnoreCollisionsTemporarilyCoroutine(collision.otherCollider));
        }
    }

    private IEnumerator IgnoreCollisionsTemporarilyCoroutine(Collider2D otherCollider)
    {
        Physics2D.IgnoreCollision(col, otherCollider, true);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(col, otherCollider, false);
    }

    private void PlayEffects(float attackForce, Vector2 point)
    {
        float particleScale = MathHelper.ClampAndMap(attackForce / originalAttackForce, 0f, 1f, 0f, 1.5f);
        particles.transform.parent.position = point;
        particles.transform.parent.localScale = Vector3.one * particleScale;
        particles.transform.parent.up = -targetDirection;
        particles.gameObject.SetActive(true);
        particles.Play();
        StartCoroutine(TurnOffParticlesCoroutine());

        float shakeIntensity = MathHelper.ClampAndMap(attackForce / originalAttackForce, 0f, 1f, 0f, 0.5f);
        ShakeCamera.Instance.Shake(shakeIntensity, -transform.up);
        GameManager.Instance.SoundManager.PlayAudioClipByEntryNameWithRandomPitch("bat_attack", 0.8f, 1.2f);
    }

    private IEnumerator TurnOffParticlesCoroutine()
    {
        yield return new WaitForSeconds(particles.main.duration);
        particles.gameObject.SetActive(false);
    }

    public void SetRotationFromDirection(Vector2 direction)
    {
        transform.rotation = transform.parent.rotation
            * aimBaseRotation
            * Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    public Vector2 targetDirection;
    public void Swing(Vector2 targetDirection)
    {
        if (isSwinging)
        {
            return;
        }
        isSwinging = true;
        isAiming = false;
        this.targetDirection = targetDirection;
        StartCoroutine(SwingCoroutine());
    }
    private IEnumerator SwingCoroutine()
    {
        // Swinging
        {
            col.enabled = true;
            swingTime = 0f;
            var sourceScale = originalScale * aimScale;
            var targetScale = originalScale * swingScale;
            var sourceRotation = transform.localRotation;
            var targetRotation = sourceRotation * Quaternion.AngleAxis(180f, Vector3.right);

            while (swingTime < swingDuration)
            {
                var factor = swingTime / swingDuration;

                var currentAngle = factor * 180f;
                var rotation = Quaternion.AngleAxis(currentAngle, Vector3.right);
                transform.localRotation = sourceRotation * rotation;
                var scale = Vector3.Lerp(sourceScale, targetScale, factor);
                transform.localScale = scale;

                attackForce = originalAttackForce * factor;

                swingTime += Time.deltaTime;
                yield return null;
            }
            transform.localRotation = targetRotation;
            transform.localScale = targetScale;
        }

        // Hitting
        {
            attackTime = 0f;
            attackForce = originalAttackForce;
            while (attackTime < attackDuration)
            {
                var factor = attackTime / attackDuration;
                attackForce = originalAttackForce * (1f - factor);
                attackTime += Time.deltaTime;
                yield return null;
            }
            attackForce = 0f;
            col.enabled = false;
        }


        // Recover
        {
            coolDownTime = 0f;
            var sourceRotation = transform.localRotation;
            var targetRotation = originalRotation;
            var sourceScale = transform.localScale;
            var targetScale = originalScale;
            var sourceColor = originalColor;
            var targetColor = originalColor;
            targetColor.a = 0.8f;
            while (coolDownTime < coolDownDuration)
            {
                var factor = coolDownTime / coolDownDuration;

                var rotation = Quaternion.Slerp(sourceRotation, targetRotation, factor);
                transform.localRotation = rotation;
                var scale = Vector3.Lerp(sourceScale, targetScale, factor);
                transform.localScale = scale;
                var color = Color.Lerp(sourceColor, targetColor, factor);
                spriteRenderer.color = color;


                coolDownTime += Time.deltaTime;
                yield return null;
            }
            transform.rotation = originalRotation;
            transform.localScale = originalScale;
            spriteRenderer.color = originalColor;
        }

        isSwinging = false;
    }

    public void StartAim()
    {
        isAiming = true;
        spriteRenderer.color = originalColor;
        col.enabled = true;
        transform.localScale = originalScale * aimScale;
    }
    

    public CinemachineBrain cinemachineBrain;
    public LineRenderer mouseAimLineRenderer;

    internal void StartAimMouse(Vector2 originalMousePosition)
    {
        mouseAimLineRenderer.gameObject.SetActive(true);
        mouseAimLineRenderer.positionCount = 2;
        var originalMousePositionWorld = cinemachineBrain.OutputCamera.ScreenToWorldPoint(originalMousePosition);
        originalMousePositionWorld.z = 0f;
        mouseAimLineRenderer.SetPosition(0, originalMousePositionWorld);
    }

    internal void SetRotationFromDirectionForMouse(Vector2 latestMousePosition, Vector2 originalMousePosition)
    {
        var mouseDelta = latestMousePosition - originalMousePosition;
        var lastMouseAimDirection = Vector2.Normalize(mouseDelta);
        SetRotationFromDirection(-lastMouseAimDirection);
        
        var originalMousePositionWorld = cinemachineBrain.OutputCamera.ScreenToWorldPoint(originalMousePosition);
        originalMousePositionWorld.z = 0f;
        mouseAimLineRenderer.SetPosition(0, originalMousePositionWorld);

        var mousePositionWorld = cinemachineBrain.OutputCamera.ScreenToWorldPoint(latestMousePosition);
        mousePositionWorld.z = 0f;
        mouseAimLineRenderer.SetPosition(1, mousePositionWorld);
    }

    public void SwingForMouse(Vector2 latestMouseAimDirection)
    {
        if (isSwinging)
        {
            return;
        }
        isSwinging = true;
        isAiming = false;
        targetDirection = latestMouseAimDirection;
        StartCoroutine(SwingCoroutine());
        StartCoroutine(DisableLineRendererCoroutine());
    }

    private IEnumerator DisableLineRendererCoroutine()
    {
        var duration = swingDuration;
        var time = 0f;
        var position0 = mouseAimLineRenderer.GetPosition(0);
        var position1 = mouseAimLineRenderer.GetPosition(1);
        var distance = Vector3.Distance(position0, position1);
        while (time < duration)
        {
            var factor = 1f - AnimationHelper.EaseOutQubic(time / duration);
            var currentDistance = Mathf.Lerp(distance, 0f, factor);
            var direction = (position1 - position0).normalized;
            mouseAimLineRenderer.SetPosition(1, position0 + direction * currentDistance);
            time += Time.deltaTime;
            yield return null;
        }
        mouseAimLineRenderer.gameObject.SetActive(false);
    }
}
