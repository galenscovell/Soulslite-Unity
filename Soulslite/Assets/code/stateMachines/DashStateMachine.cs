using UnityEngine;


public class DashStateMachine : StateMachineBehaviour
{
    private DashTrail dashTrail;
    private BaseEntity entity;

    public float dashSpeed;


    public void Setup(DashTrail trail, BaseEntity e)
    {
        dashTrail = trail;
        entity = e;
    }

    public void Interrupt(Animator animator)
    {
        dashTrail.SetEnabled(false);
        entity.nextVelocity = Vector2.zero;
        animator.SetTrigger("Dash");
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        dashTrail.SetEnabled(true);
        entity.speedMultiplier = dashSpeed;
        entity.nextVelocity = entity.facingDirection * entity.speedMultiplier;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime >= 0.2f)
        {
            dashTrail.SetEnabled(false);
            entity.nextVelocity = Vector2.zero;
        }

        if (stateTime >= 0.9f)
        {
            animator.SetTrigger("Dash");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Exit DASH");
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
