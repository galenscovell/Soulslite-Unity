using UnityEngine;


public class PlayerRanged : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerRanged.PlayerRanged");
    private PlayerAgent player;
    private PlayerGunLimb gunLimb;
    private int sfxIndex;

    public int GetHash()
    {
        return hash;
    }

    public void LoadGun()
    {
        player.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
    }

    public void Setup(PlayerAgent playerEntity, PlayerGunLimb gun, LineRenderer line, int assignedSfxIndex)
    {
        player = playerEntity;
        gunLimb = gun;
        sfxIndex = assignedSfxIndex;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.DisableMotion();
        gunLimb.Activate();

        gunLimb.UpdateTransform(player.facingDirection);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gunLimb.UpdateTransform(player.facingDirection);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gunLimb.Deactivate();
        player.EnableMotion();
    }
}
