using UnityEngine;


public class PlayerRangedStart : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerRanged.PlayerRangedStart");
    private PlayerAgent player;
    private int sfxIndex;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(PlayerAgent playerEntity, int assignedSfxIndex)
    {
        player = playerEntity;
        sfxIndex = assignedSfxIndex;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.DisableMotion();
        player.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.EnableMotion();
    }
}
