using System;

public enum AttackStateType
{
    Idle,
    CoolDown,
    Anticipation,
    Attack,
    Recovery
}

[Serializable]
public class AttackState
{
    public AttackStateType state;
    public float coolDownDuration = 1f;
    public float coolDownTime = 0f;
    public float anticipationDuration = 0.5f;
    public float anticipationTime = 0f;
    public float attackDuration = 1f;
    public float attackTime = 0f;
    public float recoveryDuration = 0.5f;
    public float recoveryTime = 0f;

    public Action onIdle;
    public Action onCoolDown;
    public Action onAnticipation;
    public Action onAttack;
    public Action onRecovery;
    public bool canAttack = true;

    public void Update(float deltaTime)
    {
        switch(state)
        {
            case AttackStateType.CoolDown:
                if (coolDownTime < coolDownDuration)
                {
                    coolDownTime += deltaTime;
                    return;
                }

                if (!canAttack)
                {
                    return;
                }
                state = AttackStateType.Anticipation;
                onAnticipation();
                coolDownTime = 0f;
                break;
            case AttackStateType.Anticipation:
                if (anticipationTime < anticipationDuration)
                {
                    anticipationTime += deltaTime;
                    return;
                }
                state = AttackStateType.Attack;
                onAttack();
                anticipationTime = 0f;
                break;
            case AttackStateType.Attack:
                if (attackTime < attackDuration)
                {
                    attackTime += deltaTime;
                    return;
                }
                state = AttackStateType.Recovery;
                onRecovery();
                attackTime = 0f;
                break;
            case AttackStateType.Recovery:
                if (recoveryTime < recoveryDuration)
                {
                    recoveryTime += deltaTime;
                    return;
                }
                state = AttackStateType.CoolDown;
                onCoolDown();
                recoveryTime = 0f;
                break;
            default:
                break;
        }
    }

    public void EnterAttackState()
    {
        onCoolDown();
        state = AttackStateType.CoolDown;
    }

    public void ExitAttackState()
    {
        onIdle();
        state = AttackStateType.Idle;
        coolDownTime = 0f;
        anticipationTime = 0f;
        attackTime = 0f;
        recoveryTime = 0f;
    }
}