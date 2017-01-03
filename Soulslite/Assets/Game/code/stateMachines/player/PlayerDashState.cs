using UnityEngine;


public class PlayerDashState : StateMachineBehaviour
{
    private PlayerAgent player;
    private DashTrail dashTrail;
    private AudioSource dashSound;
    
    private int chainCounter = 0;
    private float dashSpeed = 800f;

    private float animationSpeed = 1f;
    private float maxAnimationSpeed = 1.5f;

    private bool chainableState = false;


    public void Setup(PlayerAgent playerEntity, DashTrail trail, AudioSource sound)
    {
        player = playerEntity;
        dashTrail = trail;
        dashSound = sound;
    }

    public void Interrupt(Animator animator)
    {
        animator.speed = 1f;
        dashSound.pitch = 1f;
        chainCounter = 0;
        dashTrail.SetEnabled(false);
        animator.SetBool("Dashing", false);
    }

    public void Chain(Animator animator, Vector2 direction)
    {
        if (chainableState)
        {
            chainCounter++;
            player.facingDirection = direction;
            animator.Play("PlayerDashState", -1, 0f);
        }
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        dashTrail.SetEnabled(true);

        float newSpeed;
        if (chainCounter < 5)
        {
            animationSpeed = 1 + (chainCounter * 0.1f);
            dashSound.pitch = 1 + (chainCounter * 0.075f);
            newSpeed = dashSpeed + (dashSpeed * (chainCounter * 0.075f));
        }
        else
        {
            animationSpeed = maxAnimationSpeed;
            dashSound.pitch = 1.375f;
            newSpeed = 1250f;
        }

        animator.speed = animationSpeed;
        player.SetSpeed(newSpeed);

        player.SetNextVelocity(player.facingDirection * player.GetSpeed());
        chainableState = false;

        dashSound.Play();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (player.AbleToMove() && stateTime >= 0.2f)
        {
            player.DisableMotion();
            player.SetSpeed(player.GetNormalSpeed());
        }
        else if (stateTime >= 0.6f && stateTime < 1f)
        {
            chainableState = true;
        }
        else if (stateTime >= 1)
        {
            animator.speed = 1f;
            dashSound.pitch = 1f;
            chainCounter = 0;
            dashTrail.SetEnabled(false);
            animator.SetBool("Dashing", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        chainableState = false;
        player.EnableMotion();
    }
}
