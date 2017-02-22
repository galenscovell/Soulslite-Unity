using UnityEngine;


public class PlayerDash : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerDash");
    private PlayerAgent player;
    private DashTrail dashTrail;
    private LineRenderer dashLine;

    private int chainCounter = 0;
    private float dashSpeed = 700f;

    private bool chainableState = false;
    private bool preventChain = false;

    private float currentPitch;
    private int sfxIndex;
    private bool sfxPlayed;


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
        animator.SetBool("Dashing", false);
    }

    public void Chain(Animator animator, Vector2 direction)
    {
        // If dash input is received within the "chainable state" time a dash chain occurs
        if (chainableState && !player.PlayerIsFalling())
        {
            chainCounter++;
            player.SetFacingDirection(direction);
            UISystem.uiSystem.UpdateStamina(-0.25f);
            animator.Play(hash, -1, 0f);
        }
        // If input is received any other time, the chain becomes locked
        else
        {
            preventChain = true;
        }
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sfxPlayed = false;
        DustSystem.dustSystem.SpawnDust(player.GetBody().position, player.GetFacingDirection());

        dashTrail.SetEnabled(true);
        dashLine.enabled = true;
        CameraSystem.cameraSystem.SetDampTime(0.3f);

        if (chainCounter < 6)
        {
            currentPitch = 1 + (chainCounter * 0.1f);
        }
        else
        {
            currentPitch = 1.6f;
        }

        player.SetSpeed(dashSpeed);
        player.SetNextVelocity(player.GetFacingDirection() * player.GetSpeed());

        preventChain = false;
        chainableState = false;
        
        BeginDashLine();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UpdateDashLine();

        float stateTime = stateInfo.normalizedTime;
        if (player.AbleToMove() && stateTime >= 0.25f)
        {
            dashTrail.SetEnabled(false);
            player.DisableMotion();
            player.RestoreDefaultSpeed();
        }
        else if (stateTime > 0.25f && player.PlayerIsFalling())
        {
            player.Fall();
            return;
        }
        else if (stateTime > 0.3f && stateTime < 0.325f)
        {
            // This is the brief window for dash chaining input
            if (!sfxPlayed)
            {
                player.PlaySfxRandomPitch(sfxIndex, currentPitch - 0.05f, currentPitch + 0.05f, 1f);
                sfxPlayed = true;
            }
            
            if (!preventChain)
            {
                chainableState = true;
            }
        }
        else if (stateTime >= 1)
        {
            animator.speed = 1f;
            currentPitch = 1f;
            chainCounter = 0;
            animator.SetBool("Dashing", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        dashTrail.SetEnabled(false);
        dashLine.enabled = false;
        preventChain = false;
        chainableState = false;
        player.EnableMotion();
        CameraSystem.cameraSystem.RestoreDefaultDampTime();
    }



    private void BeginDashLine()
    {
        dashLine.SetPosition(0, player.GetBody().position + new Vector2(0, 8));
        Color c = dashLine.material.color;
        dashLine.material.color = new Color(c.r, c.g, c.b, 1);
    }

    private void UpdateDashLine()
    {
        dashLine.SetPosition(1, player.GetBody().position + new Vector2(0, 8));

        Color c = dashLine.material.color;
        c.a -= 0.035f;
        if (c.a <= 0)
        {
            dashLine.enabled = false;
        }
        else
        {
            dashLine.material.color = c;
        }
    }
}
