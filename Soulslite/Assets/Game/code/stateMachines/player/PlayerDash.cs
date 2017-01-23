using UnityEngine;


public class PlayerDash : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerDash");
    private PlayerAgent player;
    private DashTrail dashTrail;
    private LineRenderer dashLine;

    private int chainCounter = 0;
    private float dashSpeed = 700f;

    private float animationSpeed = 1f;
    private float maxAnimationSpeed = 1.5f;

    private bool chainableState = false;

    private float currentPitch;
    private int sfxIndex;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(PlayerAgent playerEntity, DashTrail trail, LineRenderer line, int assignedSfxIndex)
    {
        player = playerEntity;
        dashTrail = trail;
        dashLine = line;
        dashLine.sortingLayerName = "Foreground";
        sfxIndex = assignedSfxIndex;
    }

    public void Interrupt(Animator animator)
    {
        animator.speed = 1f;
        currentPitch = 1f;
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
            animator.Play(hash, -1, 0f);
        }
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        dashTrail.SetEnabled(true);
        dashLine.enabled = true;
        CameraController.cameraController.SetDampTime(0.3f);

        float newSpeed;
        if (chainCounter < 5)
        {
            animationSpeed = 1 + (chainCounter * 0.1f);
            currentPitch = 1 + (chainCounter * 0.12f);
            newSpeed = dashSpeed + (dashSpeed * (chainCounter * 0.05f));
        }
        else
        {
            animationSpeed = maxAnimationSpeed;
            currentPitch = 1.6f;
            newSpeed = 900f;
        }

        animator.speed = animationSpeed;
        player.SetSpeed(newSpeed);

        player.SetNextVelocity(player.facingDirection * player.GetSpeed());
        chainableState = false;

        player.PlaySfx(sfxIndex, currentPitch, 1f);

        BeginDashLine();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UpdateDashLine();

        float stateTime = stateInfo.normalizedTime;
        if (player.AbleToMove() && stateTime >= 0.275f)
        {
            player.DisableMotion();
            player.RestoreDefaultSpeed();
        }
        else if (stateTime >= 0.5f && stateTime < 0.8f)
        {
            chainableState = true;
        }
        else if (stateTime >= 1)
        {
            animator.speed = 1f;
            currentPitch = 1f;
            chainCounter = 0;
            dashTrail.SetEnabled(false);
            dashLine.enabled = false;
            animator.SetBool("Dashing", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        chainableState = false;
        player.EnableMotion();
        CameraController.cameraController.RestoreDefaultDampTime();
    }

    private void BeginDashLine()
    {
        dashLine.SetPosition(0, player.GetBody().position + new Vector2(0, 8));
    }

    private void UpdateDashLine()
    {
        dashLine.SetPosition(1, player.GetBody().position + new Vector2(0, 8));
    }
}
