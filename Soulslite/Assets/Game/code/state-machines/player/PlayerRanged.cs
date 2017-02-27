using UnityEngine;


public class PlayerRanged : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerRanged.PlayerRanged");
    private PlayerAgent player;
    private PlayerGunLimb gunLimb;

    public int GetHash()
    {
        return hash;
    }

    public void Setup(PlayerAgent playerEntity, PlayerGunLimb gun)
    {
        player = playerEntity;
        gunLimb = gun;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.DisableMotion();
        gunLimb.Activate();

        gunLimb.UpdateTransform(player.GetFacingDirection());
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gunLimb.UpdateTransform(player.GetFacingDirection());
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gunLimb.Deactivate();
        player.EnableMotion();
    }
}
