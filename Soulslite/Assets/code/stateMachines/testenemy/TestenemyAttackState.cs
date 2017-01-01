using UnityEngine;


public class TestenemyAttackState : StateMachineBehaviour
{
    private Enemy enemy;

    private bool targeted = false;
    private bool prepared = false;
    private bool reset = false;


    public void Setup(Enemy e)
    {
        enemy = e;
    }

    public void Interrupt(Animator animator)
    {
        enemy.nextVelocity = Vector2.zero;
        animator.SetTrigger("Attack");
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.DisableMotion();
        targeted = false;
        prepared = false;
        reset = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (!targeted && stateTime >= 0.15f && stateTime < 0.2f)
        {
            enemy.directionToTarget = (enemy.TrackTarget() - enemy.GetBody().position).normalized;
            targeted = true;
        }
        else if (!prepared && stateTime >= 0.6f && stateTime < 0.65f)
        {
            enemy.EnableMotion();
            enemy.SetSpeed(220f);
            enemy.nextVelocity = enemy.directionToTarget * enemy.GetSpeed();
            prepared = true;
        }
        else if (!reset && stateTime >= 0.75f && stateTime < 0.8f)
        {
            enemy.SetSpeed(enemy.GetNormalSpeed());
            enemy.DisableMotion();
            reset = true;
        }
        else if (stateTime >= 1)
        {
            animator.SetTrigger("Attack");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableMotion();
    }
}
