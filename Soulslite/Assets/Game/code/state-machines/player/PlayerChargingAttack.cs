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
        player.DisableMotion();
        attackReady = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.25f && stateTime < 0.5f)
        {
            if (!AttackIsReady())
            {
                player.PlaySfxRandomPitch(sfxIndex, 0.8f, 1.2f, 1f);
                player.StartCoroutine(player.FlashSpriteColor(flashColor, 0.4f, 0.6f, 0.2f, 0));
                attackReady = true;
            }
        }

        if (AttackIsReady() && stateTime > 2f)
        {
            animator.SetBool("ChargedAttack", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("ChargeHeldTime", 0);
        player.EnableMotion();
    }
}
