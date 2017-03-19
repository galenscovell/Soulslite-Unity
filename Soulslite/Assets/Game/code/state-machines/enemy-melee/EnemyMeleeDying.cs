using UnityEngine;


public class EnemyMeleeDying : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.EnemyMeleeDying");
    private Enemy enemy;
    private Vector2 flungVelocity;
    private int sfxIndex;

    private bool flung;
    private bool fallen;


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
        enemy.IgnoreEntityCollisions();
        enemy.EnableFlippedX();

        flung = false;
        fallen = false;

        enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.1f, 1);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime < 0.4f)
        {
            if (!flung)
            {
                TimeSystem.timeSystem.SlowTime(0f, 0.1f);
                enemy.SetMovementImpulse(flungVelocity, 4, 0.25f);
                flung = true;
            }
        }
        else if (stateTime > 0.7f && stateTime < 1)
        {
            if (!fallen)
            {
                enemy.GetShadow().LerpScale(new Vector2(1.5f, 1.5f), 0.2f);
                fallen = true;
            }
        }
        else if (stateTime > 1)
        {
            enemy.EnableDeath();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
