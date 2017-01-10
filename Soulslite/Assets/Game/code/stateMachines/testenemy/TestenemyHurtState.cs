using UnityEngine;


public class TestenemyHurtState : StateMachineBehaviour
{
    private TestEnemyAgent enemy;
    private Vector2 flungVelocity;


    public void Setup(TestEnemyAgent e)
    {
        enemy = e;
    }

    public void SetFlungVelocity(Vector2 velocity)
    {
        flungVelocity = velocity;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableFlippedX();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (stateTime < 0.45f)
        {
            enemy.SetSpeed(180f);
            enemy.SetNextVelocity(flungVelocity);
        }
        else if (stateTime >= 0.45f && stateTime < 1)
        {
            enemy.SetSpeed(enemy.defaultSpeed);
            enemy.DisableMotion();
        }
        else if (stateTime >= 1)
        {
            enemy.Die();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableMotion();
    }
}
