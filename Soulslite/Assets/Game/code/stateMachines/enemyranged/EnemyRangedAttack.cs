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
        gunLimb.Activate();

        shotOne = false;
        shotTwo = false;
        shotThree = false;

        Vector2 positionDiff = enemy.GetTarget().position - enemy.GetBody().position;
        gunLimb.UpdateGunLimb(positionDiff);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.12f && stateTime < 0.15f)
        {
            if (!shotOne)
            {
                enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
                shotOne = true;
                BulletSystem.bulletSystem.SpawnBullet(gunLimb.GetBarrelPosition(), enemy.GetFacingDirection(), "EnemyBulletTag", "EnemyBulletLayer");
            }
        }
        else if (stateTime > 0.32f && stateTime < 0.35f)
        {
            if (!shotTwo)
            {
                enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
                shotTwo = true;
                BulletSystem.bulletSystem.SpawnBullet(gunLimb.GetBarrelPosition(), enemy.GetFacingDirection(), "EnemyBulletTag", "EnemyBulletLayer");
            }
        }
        else if (stateTime > 0.52f && stateTime < 0.55f)
        {
            if (!shotThree)
            {
                enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
                shotThree = true;
                BulletSystem.bulletSystem.SpawnBullet(gunLimb.GetBarrelPosition(), enemy.GetFacingDirection(), "EnemyBulletTag", "EnemyBulletLayer");
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
