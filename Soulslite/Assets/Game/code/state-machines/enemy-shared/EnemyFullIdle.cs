using UnityEngine;


public class EnemyFullIdle : StateMachineBehaviour
{
    public string stateName;

    private int hash;
    private Enemy enemy;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e)
    {
        hash = Animator.StringToHash(stateName);
        enemy = e;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.DisableMotion();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 1)
        {
            animator.SetBool("Idling", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableMotion();
    }
}
