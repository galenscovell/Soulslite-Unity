using UnityEngine;


public class TestenemyAttackState : StateMachineBehaviour
{
    private Enemy enemy;
    private AudioSource attackSound;

    private bool targeted = false;
    private bool prepared = false;
    private bool reset = false;

    // Denotes when this state can be interrupted
    private bool vulnerable = true;


    public void Setup(Enemy e, AudioSource sound)
    {
        enemy = e;
        attackSound = sound;
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
        else if (!prepared && stateTime >= 0.6f && stateTime < 0.65f)
        {
            enemy.EnableMotion();
            enemy.SetSpeed(220f);
            enemy.SetNextVelocity(enemy.directionToTarget * enemy.GetSpeed());
            prepared = true;
        }
        else if (!reset && stateTime >= 0.75f && stateTime < 0.8f)
        {
            enemy.SetSpeed(enemy.GetNormalSpeed());
            enemy.DisableMotion();
            reset = true;
            attackSound.PlayOneShot(attackSound.clip);
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
