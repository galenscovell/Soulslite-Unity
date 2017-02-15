using UnityEngine;


public class EnemyRangedAttack : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.EnemyRangedAttack");
    private Enemy enemy;
    private EnemyRangedGunLimb gunLimb;
    private int sfxIndex;
    private bool sfxPlayed;

    // Denotes when this state can be interrupted
    private bool vulnerable = true;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e, EnemyRangedGunLimb gun, int assignedSfxIndex)
    {
        enemy = e;
        gunLimb = gun;
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
        sfxPlayed = false;
        Vector2 positionDiff = enemy.GetTarget().position - enemy.GetBody().position;

        enemy.DisableMotion();
        gunLimb.Activate();

        gunLimb.UpdateGunLimb(positionDiff);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.3f && stateTime < 1)
        {
            if (!sfxPlayed)
            {
                enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
                sfxPlayed = true;
            }
        }
        else if (stateTime >= 1)
        {
            animator.SetBool("Attacking", false);
        }
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gunLimb.Deactivate();
        enemy.EnableMotion();
    }
}
