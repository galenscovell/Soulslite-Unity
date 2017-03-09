using UnityEngine;


public class PlayerDash : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerDash.PlayerDash");
    private PlayerAgent player;
    private LineRenderer dashLine;

    private int chainCounter = 0;
    private float dashSpeed = 960f;

    private bool chainableState;
    private bool preventChain;
    private bool slowed;

    private float currentPitch;
    private int[] sfx;

    private float fxRate = 0.05f;
    private float fxCounter;
    private float sfxRate = 0.1f;
    private float sfxCounter;
    
    public Color flashColor;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(PlayerAgent playerEntity, LineRenderer line, int[] assignedSfx)
    {
        player = playerEntity;
        dashLine = line;
        dashLine.sortingLayerName = "Foreground";
        sfx = assignedSfx;
    }

    public void Interrupt(Animator animator)
    {
        animator.speed = 1f;
        currentPitch = 1f;
        chainCounter = 0;
        animator.SetBool("Dashing", false);
        animator.SetInteger("DashChain", 0);
    }

    public void Chain(Animator animator, Vector2 direction)
    {
        // If dash input is received within the "chainable state" time a dash chain occurs
        if (chainableState && !player.PlayerIsFalling())
        {
            chainCounter++;
            player.SetFacingDirection(direction);
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
        animator.SetInteger("DashChain", animator.GetInteger("DashChain") + 1);

        TrailSystem.trailSystem.BeginTrail();
        DustSystem.dustSystem.SpawnDust(player.GetBody().position, player.GetFacingDirection());
        CameraSystem.cameraSystem.SetDampTime(0.3f);

        if (chainCounter < 6) currentPitch = 1 + (chainCounter * 0.1f);
        else currentPitch = 1.6f;

        player.SetSpeed(dashSpeed);
        player.SetNextVelocity(player.GetFacingDirection() * player.GetSpeed());

        preventChain = false;
        chainableState = false;
        slowed = false;
        fxCounter = fxRate;
        sfxCounter = sfxRate;

        BeginDashLine();

        player.PlaySfxRandomPitch(sfx[0], currentPitch - 0.05f, currentPitch + 0.05f, 1f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UpdateDashLine();

        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.3f)
        {
            // Fall once dash main movement stops
            if (player.PlayerIsFalling())
            {
                Interrupt(animator);
                player.Fall();
                return;
            }
        }

        if (stateTime > 0.125f && stateTime < 0.2f)
        {
            player.SetSpeed(player.GetDefaultSpeed() / 2);
        }
        else if (stateTime > 0.4f && stateTime < 0.45f)
        {
            TrailSystem.trailSystem.EndTrail();
        }
        else if (stateTime > 0.5f && stateTime < 0.9f)
        {
            // Chain input period
            chainableState = true;
        }

        if (stateTime > 0.6f && stateTime < 1)
        {
            if (stateTime > 0.9f)
            {
                chainableState = false;
                player.SetInput(false);
            }

            fxCounter += Time.deltaTime;
            sfxCounter += Time.deltaTime;
            if (fxCounter > fxRate)
            {
                DustSystem.dustSystem.SpawnDust(player.GetBody().position, player.GetFacingDirection());
                fxCounter = 0;
            }

            if (sfxCounter > sfxRate)
            {
                player.PlaySfxRandomPitch(sfx[1], 0.7f, 1.4f, 0.1f);
                sfxCounter = 0;
            }
        }
        else if (stateTime > 1)
        {
            animator.speed = 1f;
            currentPitch = 1f;
            chainCounter = 0;
            animator.SetBool("Dashing", false);
            animator.SetInteger("DashChain", 0);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.RestoreDefaultSpeed();
        preventChain = false;
        dashLine.enabled = false;
        player.SetInput(true);
        CameraSystem.cameraSystem.RestoreDefaultDampTime();
    }



    private void BeginDashLine()
    {
        dashLine.enabled = true;
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
