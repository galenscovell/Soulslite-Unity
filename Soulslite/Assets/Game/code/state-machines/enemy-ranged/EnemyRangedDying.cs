using UnityEngine;


public class EnemyRangedDying : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.EnemyRangedDying");
    private Enemy enemy;
    private Vector2 flungVelocity;
    private int sfxIndex;

    private bool flung;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e, int assignedSfxIndex)
    {
        enemy = e;
        sfxIndex = assignedSfxIndex;
    }

    public void SetFlungVelocity(Vector2 velocity)
    {
        flungVelocity = velocity;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.DisableMotion();
        enemy.IgnoreEntitiesPhysics();
        enemy.EnableFlippedX();
        flung = false;
        
        enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.1f, 1);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime < 0.2f)
        {
            if (!flung)
            {
                enemy.SetMovementImpulse(flungVelocity, 3, 0.25f);
                flung = true;
            }
        }
        else if (stateTime > 1)
        {
            enemy.EnableDeath();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableMotion();
    }
}
