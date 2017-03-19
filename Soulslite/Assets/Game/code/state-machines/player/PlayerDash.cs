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

    private float runtime;
    private float currentPitch;
    private int[] sfx;

    private float fxRate = 0.075f;
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



    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BeginDashLine();
        player.GetShadow().TurnOff();

        animator.SetInteger("DashChain", animator.GetInteger("DashChain") + 1);

        CameraSystem.cameraSystem.SetDampTime(0.3f);
        DustSystem.dustSystem.SpawnDust(player.GetBody().position, player.GetFacingDirection());
        TrailSystem.trailSystem.BeginTrail();

        if (chainCounter < 6) currentPitch = 1 + (chainCounter * 0.1f);
        else currentPitch = 1.6f;

        if (animator.GetInteger("DashChain") > 2) runtime = 2f;
        else runtime = 1;

        player.DisableMotion();
        player.SetSpeed(dashSpeed);

        preventChain = false;
        chainableState = false;
        slowed = false;
        fxCounter = fxRate;
        sfxCounter = sfxRate;
        
        player.PlaySfxRandomPitch(sfx[0], currentPitch - 0.05f, currentPitch + 0.05f, 1f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UpdateDashLine();

        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.3f)
        {
            // Fall once dash main movement stops
            if (player.IsFalling())
            {
                End(animator);
                CameraSystem.cameraSystem.RestoreDefaultDampTime();
                player.Fall();
                return;
            }
        }

        if (stateTime > 0.1f && stateTime < 0.25f)
        {
            player.IgnoreEntityCollisions();
            player.EnableMotion();
            player.SetNextVelocity(player.GetFacingDirection() * player.GetSpeed());
        }
        else if (stateTime > 0.25f && stateTime < 0.3f)
        {
            player.SetSpeed(player.GetDefaultSpeed() / 2);
            player.StartCoroutine(player.FlashSpriteColor(flashColor, 0.1f, 1f, 0.4f, 0));
        }
        else if (stateTime > 0.5f && stateTime < 0.6f)
        {
            // Allow chain input if dash not attempted prior
            if (!preventChain)
            {
                chainableState = true;
            }

            player.GetShadow().TurnOn();
            TrailSystem.trailSystem.EndTrail();
            player.RestoreCollisions();
        }
        if (stateTime > 0.8f && stateTime < runtime)
        {
            chainableState = false;

            fxCounter += Time.deltaTime;
            if (fxCounter > fxRate)
            {
                DustSystem.dustSystem.SpawnDust(player.GetBody().position, player.GetFacingDirection());
                fxCounter = 0;
            }

            if (runtime > 1)
            {
                sfxCounter += Time.deltaTime;
                if (sfxCounter > sfxRate)
                {
                    player.PlaySfxRandomPitch(sfx[1], 0.7f, 1.4f, 0.15f);
                    sfxCounter = 0;
                }
            }
        }
        else if (stateTime > runtime)
        {
            CameraSystem.cameraSystem.RestoreDefaultDampTime();
            End(animator);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EndDashLine();

        player.EnableMotion();
        player.RestoreDefaultSpeed();
    }



    public void Chain(Animator animator, Vector2 direction)
    {
        // If dash input is received within the "chainable state" time a dash chain occurs
        if (chainableState && !player.IsFalling())
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

    public void End(Animator animator)
    {
        currentPitch = 1f;
        chainCounter = 0;
        animator.SetInteger("DashChain", 0);
        animator.SetBool("Dashing", false);
        player.GetShadow().TurnOn();
    }

    private void BeginDashLine()
    {
        dashLine.SetPosition(0, player.GetBody().position + new Vector2(0, 8));
        dashLine.SetPosition(1, player.GetBody().position + new Vector2(0, 8));
        Color c = dashLine.material.color;
        dashLine.material.color = new Color(c.r, c.g, c.b, 1);
        dashLine.enabled = true;
    }

    private void EndDashLine()
    {
        dashLine.enabled = false;
    }

    private void UpdateDashLine()
    {
        dashLine.SetPosition(1, player.GetBody().position + new Vector2(0, 8));

        Color c = dashLine.material.color;
        c.a -= 0.035f;
        if (c.a <= 0) EndDashLine();
        else dashLine.material.color = c;
    }
}
