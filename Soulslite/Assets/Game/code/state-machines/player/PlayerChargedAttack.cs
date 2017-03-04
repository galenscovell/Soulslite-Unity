using UnityEngine;


public class PlayerChargedAttack : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerChargeAttack.ChargedAttack");
    private int interruptHash = Animator.StringToHash("Base Layer.PlayerAttackInterrupt");
    private PlayerAgent player;
    private int sfxIndex;

    // Denotes when this state can be interrupted
    private bool vulnerable = true;
    private bool moved;
    private bool interrupted;


    public int GetHash()
    {
        return hash;
    }

    public bool Interrupt(Animator animator)
    {
        if (vulnerable)
        {
            interrupted = true;
            return true;
        }
        return false;
    }

    public bool IsVulnerable()
    {
        return vulnerable;
    }

    public void Setup(PlayerAgent playerEntity, int assignedSfxIndex)
    {
        player = playerEntity;
        sfxIndex = assignedSfxIndex;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        moved = false;
        player.DisableMotion();
        player.PlaySfxRandomPitch(sfxIndex, 0.8f, 1.2f, 1f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.5f && stateTime < 1)
        {
            if (!moved)
            {
                player.SetMovementImpulse(player.GetFacingDirection().normalized, 2.5f, 0.1f);
                DustSystem.dustSystem.SpawnDust(player.GetBody().position, player.GetFacingDirection());
                moved = true;
            }
        }
        else if (stateTime > 1)
        {
            animator.SetBool("ChargingAttack", false);
            animator.SetBool("ChargedAttack", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("ChargeHeldTime", 0);
        player.EnableMotion();
    }
}
