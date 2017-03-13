using UnityEngine;


public class MinibossAttack : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.MinibossAttack");
    private Enemy enemy;
    private int sfxIndex;

    private bool attacked;

    // Denotes when this state can be interrupted
    private bool vulnerable;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e, int assignedSfxIndex)
    {
        enemy = e;
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
        enemy.DisableMotion();
        attacked = false;
        vulnerable = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.15f && stateTime < 0.2f)
        {
            if (!attacked)
            {
                enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.2f, 1f);
                attacked = true;
            }
            
        }
        else if (stateTime > 1)
        {
            animator.SetBool("Attacking", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableMotion();
    }
}
