using UnityEngine;


public class EnemyRangedHurt : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.EnemyRangedHurt");
    private Enemy enemy;
    private Vector2 flungVelocity;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e)
    {
        enemy = e;
    }

    public void SetFlungVelocity(Vector2 velocity)
    {
        flungVelocity = velocity;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
