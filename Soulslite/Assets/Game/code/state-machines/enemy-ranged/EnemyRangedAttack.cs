using UnityEngine;


public class EnemyRangedAttack : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.EnemyRangedAttack");
    private Enemy enemy;
    private EnemyRangedGunLimb gunLimb;
    private int sfxIndex;

    private bool shotOne;
    private bool shotTwo;
    private bool shotThree;
    private bool resetting;

    // Denotes when this state can be interrupted
    private bool vulnerable;


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
            animator.ForceStateNormalizedTime(0.8f);
            return true;
        }
        return false;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gunLimb.Activate();

        shotOne = false;
        shotTwo = false;
        shotThree = false;
        resetting = false;
        vulnerable = false;

        Vector2 positionDiff = enemy.GetTargetBody().position - enemy.GetBody().position;
        gunLimb.UpdateGunLimb(positionDiff, enemy);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.25f && stateTime < 0.3f)
        {
            if (!shotOne)
            {
                enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
                shotOne = true;
                BulletSystem.bulletSystem.SpawnBullet(gunLimb.GetBarrelPosition(), enemy.GetFacingDirection(), "EnemyBulletTag", "EnemyBulletLayer");
            }
        }
        else if (stateTime > 0.4f && stateTime < 0.45f)
        {
            if (!shotTwo)
            {
                enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
                shotTwo = true;
                BulletSystem.bulletSystem.SpawnBullet(gunLimb.GetBarrelPosition(), enemy.GetFacingDirection(), "EnemyBulletTag", "EnemyBulletLayer");
            }
        }
        else if (stateTime > 0.55f && stateTime < 0.6f)
        {
            if (!shotThree)
            {
                enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
                shotThree = true;
                BulletSystem.bulletSystem.SpawnBullet(gunLimb.GetBarrelPosition(), enemy.GetFacingDirection(), "EnemyBulletTag", "EnemyBulletLayer");
            }
        }
        else if (stateTime > 0.8f && stateTime < 1)
        {
            if (!resetting)
            {
                gunLimb.ResetGunLimb(enemy);
                resetting = true;
            }
        }
        else if (stateTime > 1)
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
