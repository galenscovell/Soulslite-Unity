using UnityEngine;


public class PlayerRangedExit : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerRanged.PlayerRangedExit");
    private PlayerAgent player;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(PlayerAgent playerEntity)
    {
        player = playerEntity;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
