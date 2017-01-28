using UnityEngine;


public class TestenemyAttack : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.TestenemyAttack");
    private Enemy enemy;
    private int sfxIndex;

    private bool targeted = false;
    private bool prepared = false;
    private bool reset = false;

    // Denotes when this state can be interrupted
    private bool vulnerable = true;


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
        enemy.EnableDirectVelocity(true);

        enemy.DisableMotion();
        targeted = false;
        prepared = false;
        reset = false;
        vulnerable = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (!targeted && stateTime >= 0.15f && stateTime < 0.2f)
        {
            enemy.directionToTarget = (enemy.TrackTarget() - enemy.GetBody().position).normalized;
            targeted = true;
        }
        else if (!prepared && stateTime >= 0.5f && stateTime < 0.65f)
        {
            enemy.EnableMotion();
            enemy.SetSpeed(240f);
            enemy.SetNextVelocity(enemy.directionToTarget * enemy.GetSpeed());
            enemy.EnableDirectVelocity(false);
            prepared = true;
        }
        else if (!reset && stateTime >= 0.65f && stateTime < 0.8f)
        {
            enemy.RestoreDefaultSpeed();
            enemy.DisableMotion();
            enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1);
            reset = true;
        }
        else if (stateTime >= 1)
        {
            animator.SetBool("Attacking", false);
            vulnerable = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableMotion();
    }
}
