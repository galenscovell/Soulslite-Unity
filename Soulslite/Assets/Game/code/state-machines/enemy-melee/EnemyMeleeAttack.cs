using UnityEngine;


public class EnemyMeleeAttack : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.EnemyMeleeAttack");
    private Enemy enemy;
    private int sfxIndex;

    private bool targeted;
    private bool prepared;
    private bool reset;

    // Denotes when this state can be interrupted
    private bool vulnerable;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e, int assignedSfxIndex)
    {
        enemy = e;
        sfxIndex = assignedSfxIndex;
    }

    public bool Interrupt(Animator animator)
    {
        if (vulnerable)
        {
            animator.SetBool("Attacking", false);
            return true;
        }
        return false;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.DisableMotion();
        targeted = false;
        prepared = false;
        reset = false;
        vulnerable = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.15f && stateTime < 0.2f)
        {
            if (!targeted)
            {
                enemy.directionToTarget = (enemy.TrackTarget() - enemy.GetBody().position).normalized;
                targeted = true;
            }
        }
        else if (stateTime > 0.5f && stateTime < 0.65f)
        {
            if (!prepared)
            {
                enemy.EnableMotion();
                enemy.SetSpeed(240f);
                enemy.SetNextVelocity(enemy.directionToTarget * enemy.GetSpeed());
                prepared = true;
            }
        }
        else if (stateTime > 0.65f && stateTime < 0.8f)
        {
            if (!reset)
            {
                enemy.RestoreDefaultSpeed();
                enemy.DisableMotion();
                enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1);
                reset = true;
            }
        }
        else if (stateTime > 1)
        {
            animator.SetBool("Attacking", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableMotion();
    }
}
