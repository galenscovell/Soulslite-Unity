using UnityEngine;


public class PlayerChargingAttack : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerChargeAttack.Charging");
    private PlayerAgent player;
    private int sfxIndex;

    private bool attackReady;
    private Color flashColor = new Color(0.1f, 0.98f, 1f, 1);


    public int GetHash()
    {
        return hash;
    }

    public bool AttackIsReady()
    {
        return attackReady;
    }

    public void Setup(PlayerAgent playerEntity, int assignedSfxIndex)
    {
        player = playerEntity;
        sfxIndex = assignedSfxIndex;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackReady = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 1 && stateTime < 1.5)
        {
            if (!AttackIsReady())
            {
                player.PlaySfxRandomPitch(sfxIndex, 0.8f, 1.2f, 1f);
                player.StartCoroutine(player.FlashSpriteColor(flashColor, 0.45f, 0.8f, 0.15f, 0));
                attackReady = true;
            }
        }

        if (AttackIsReady() && stateTime > 5f)
        {
            animator.SetBool("ChargedAttack", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("ChargeHeldTime", 0);
    }
}
