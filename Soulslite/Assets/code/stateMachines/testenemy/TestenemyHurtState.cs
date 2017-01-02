using UnityEngine;


public class TestenemyHurtState : StateMachineBehaviour
{
    private Enemy enemy;
    private Vector2 hurtVelocity;


    public void Setup(Enemy e)
    {
        enemy = e;
    }

    public void SetHurtVelocity(Vector2 velocity)
    {
        hurtVelocity = velocity;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableMirrored();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (stateTime < 0.4f)
        {
            enemy.SetSpeed(80f);
            enemy.nextVelocity = hurtVelocity;
        }
        else if (stateTime >= 0.4f && stateTime < 1)
        {
            enemy.SetSpeed(enemy.normalSpeed);
            enemy.DisableMotion();
        }
        else if (stateTime >= 1)
        {
            animator.SetTrigger("Hurt");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.DisableMirrored();
        enemy.EnableMotion();
    }
}
