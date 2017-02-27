using UnityEngine;


public class PlayerRangedAttack : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerRanged.Shoot");
    private PlayerAgent player;
    private PlayerGunLimb gunLimb;
    private int sfxIndex;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(PlayerAgent playerEntity, PlayerGunLimb gun, int assignedSfxIndex)
    {
        player = playerEntity;
        gunLimb = gun;
        sfxIndex = assignedSfxIndex;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gunLimb.UpdateTransform(player.GetFacingDirection());
        player.DisableMotion();
        gunLimb.Activate();
        player.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
        BulletSystem.bulletSystem.SpawnBullet(gunLimb.GetBarrelPosition(), player.GetFacingDirection(), "PlayerBullet", "PlayerBulletLayer");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gunLimb.UpdateTransform(player.GetFacingDirection());

        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 1)
        {
            animator.SetBool("Attacking", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Attacking", false);
    }
}
