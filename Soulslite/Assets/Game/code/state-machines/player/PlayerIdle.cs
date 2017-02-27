using UnityEngine;


public class PlayerIdle : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerIdle");
    private PlayerAgent player;
    private float idleTime = 0f;


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
        idleTime = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        idleTime += Time.deltaTime;

        if (idleTime > 8)
        {
            animator.SetBool("FullIdle", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        idleTime = 0f;
    }
}
