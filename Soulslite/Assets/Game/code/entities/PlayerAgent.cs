using System.Collections;
using UnityEngine;


public class PlayerAgent : BaseEntity
{
    private AnimatorStateInfo currentStateInfo;

    private int attackChainWaitFrames;

    private bool inputEnabled = false;
    private bool falling = false;
    private bool chargeAttacked = false;

    // State machines
    private PlayerAttack1 attack1;
    private PlayerAttack2 attack2;
    private PlayerAttack3 attack3;
    private PlayerAttackInterrupt attackInterrupt;
    private PlayerChargingAttack chargingAttack;
    private PlayerChargedAttack chargedAttack;
    private PlayerDash dash;
    private PlayerDeath death;
    private PlayerFall fall;
    private PlayerFullIdle fullIdle;
    private PlayerHurt hurt;
    private PlayerIdle idle;
    private PlayerMovement movement;
    private PlayerRanged ranged;
    private PlayerRangedAttack rangedAttack;

    private IEnumerator heartbeat;


    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();

        PlayerGunLimb gunLimb = transform.Find("GunLimb").GetComponent<PlayerGunLimb>();

        attack1 = animator.GetBehaviour<PlayerAttack1>();
        attack2 = animator.GetBehaviour<PlayerAttack2>();
        attack3 = animator.GetBehaviour<PlayerAttack3>();
        attackInterrupt = animator.GetBehaviour<PlayerAttackInterrupt>();
        chargingAttack = animator.GetBehaviour<PlayerChargingAttack>();
        chargedAttack = animator.GetBehaviour<PlayerChargedAttack>();
        dash = animator.GetBehaviour<PlayerDash>();
        death = animator.GetBehaviour<PlayerDeath>();
        fall = animator.GetBehaviour<PlayerFall>();
        fullIdle = animator.GetBehaviour<PlayerFullIdle>();
        hurt = animator.GetBehaviour<PlayerHurt>();
        idle = animator.GetBehaviour<PlayerIdle>();
        movement = animator.GetBehaviour<PlayerMovement>();
        ranged = animator.GetBehaviour<PlayerRanged>();
        rangedAttack = animator.GetBehaviour<PlayerRangedAttack>();

        // Ints in state Setups are sfx index
        attack1.Setup(this, 0);
        attack2.Setup(this, 1);
        attack3.Setup(this, 1);
        attackInterrupt.Setup(this, 2);
        chargingAttack.Setup(this, 11);
        chargedAttack.Setup(this, 12);
        dash.Setup(this, GetComponent<DashTrail>(), GetComponent<LineRenderer>(), new int[2] { 3, 10 });
        death.Setup(this, 4);
        fall.Setup(this, 4);
        fullIdle.Setup(this);
        hurt.Setup(this, 5);
        idle.Setup(this);
        movement.Setup(this, 6);
        ranged.Setup(this, gunLimb);
        rangedAttack.Setup(this, gunLimb, 8);

        heartbeat = NearDeathHeartbeat();

        DontDestroyOnLoad(this);
    }


    /**************************
     *        Update          *
     **************************/
    private new void Update()
    {
        base.Update();

        if (inputEnabled)
        {
            // Charge attack tracking
            if (Input.GetButton("Attack"))
            {
                if (!chargeAttacked)
                {
                    if (UISystem.uiSystem.GetCurrentStamina() >= 30)
                    {
                        float chargeHeldTime = animator.GetFloat("ChargeHeldTime") + Time.deltaTime;
                        animator.SetFloat("ChargeHeldTime", chargeHeldTime);

                        if (chargeHeldTime > 0.3f && currentStateInfo.fullPathHash != chargingAttack.GetHash())
                        {
                            UISystem.uiSystem.UpdateStamina(-30);
                            chargeAttacked = true;
                            animator.SetBool("ChargingAttack", true);
                        }
                    }
                    else
                    {
                        animator.SetFloat("ChargeHeldTime", 0);
                    }
                }
            }
            else
            {
                chargeAttacked = false;

                if (!chargingAttack.AttackIsReady())
                {
                    animator.SetBool("ChargingAttack", false);
                    animator.SetFloat("ChargeHeldTime", 0);
                }
            }

            UpdateAttackChain();
            currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (currentStateInfo.fullPathHash == movement.GetHash() || currentStateInfo.fullPathHash == idle.GetHash())
            {
                DefaultUpdate();
            }
            else if (currentStateInfo.fullPathHash == dash.GetHash())
            {
                DashUpdate();
            }
            else if (currentStateInfo.fullPathHash == ranged.GetHash() ||
                currentStateInfo.fullPathHash == rangedAttack.GetHash())
            {
                RangedUpdate();
            }
            else if (currentStateInfo.fullPathHash == fullIdle.GetHash())
            {
                FullIdleUpdate();
            }
        }
    }

    private new void FixedUpdate()
    {
        // Will update body velocity and facing direction
        base.FixedUpdate();
    }


    /**************************
     *     Player States      *
     **************************/
    private void DefaultUpdate()
    {
        if (falling)
        {
            Fall();
            return;
        }

        RestoreDefaultSpeed();
        SetNextVelocity(GetAxisInput() * speed);

        // Start attack
        if (Input.GetButtonDown("Attack"))
        {
            if (animator.GetInteger("AttackChain") == 2 && attackChainWaitFrames > 0)
            {
                animator.SetInteger("AttackVersion", 3);
                attackChainWaitFrames = 0;
            }
            else
            {
                attackChainWaitFrames = 45;
            }
            animator.SetBool("Attacking", true);
        }
        // Start dash
        else if (Input.GetButtonDown("Dash"))
        {
            animator.SetBool("Dashing", true);
        }
        // Start ranged attack mode
        else if (Input.GetButton("Ranged"))
        {
            animator.SetBool("Ranged", true);
        }
    }

    private void DashUpdate()
    {
        // Chain dash
        if (Input.GetButtonDown("Dash"))
        {
            Vector2 dashDirection = GetAxisInput();
            if (dashDirection.magnitude == 0)
            {
                dashDirection = facingDirection;
            }

            dash.Chain(animator, dashDirection);
        }
    }

    private void RangedUpdate()
    {
        if (falling)
        {
            Fall();
            return;
        }

        Vector2 direction = GetAxisInput();

        // Reduce precision of axis input
        if (direction.magnitude > 0.6f)
        {
            SetFacingDirection(direction);
        }

        // Start ranged attack shot
        if (Input.GetButtonDown("Attack"))
        {
            if (UISystem.uiSystem.GetCurrentAmmo() >= 1)
            {
                UISystem.uiSystem.UpdateAmmo(-1);
                animator.SetBool("Attacking", true);
            }
            else
            {
                PlaySfxRandomPitch(7, 0.8f, 1.3f, 1);
            }
                
        }

        // End ranged attack mode when button released
        if (!Input.GetButton("Ranged"))
        {
            animator.SetBool("Attacking", false);
            animator.SetBool("Ranged", false);
        }
    }

    private void FullIdleUpdate()
    {
        if (falling)
        {
            Fall();
            return;
        }

        Vector2 direction = GetAxisInput();

        // Exit fullIdle upon any user input + update facing direction out of fullIdle
        if (direction.magnitude > 0.1f || Input.GetButtonDown("Attack") || Input.GetButtonDown("Ranged") || Input.GetButtonDown("Dash"))
        {
            SetFacingDirection(direction);
            animator.SetBool("FullIdle", false);
        }
    }


    /**************************
     *       Collisions       *
     **************************/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "TransitionTag":
                // Ignore collisions with transition zones
                return;
            case "CritterTag":
                // Ignore collisions with critters
                return;
            case "ObstacleTag":
                // Interrupt and stop attack if collided with obstacle
                if (InterruptAttack()) return;
                break;
            case "FalloffTag":
                // Set player as ready to fall if colliding with falloff boundary
                falling = true;
                return;
            case "EnemyAttackTag":
            case "EnemyBulletTag":
                // Ignore if already hurting
                if (currentStateInfo.fullPathHash == hurt.GetHash()) return;

                // Interrupt dash if currently dashing
                if (currentStateInfo.fullPathHash == dash.GetHash())
                {
                    dash.Interrupt(animator);
                }

                Vector2 collisionDirection = transform.position - collision.transform.position;
                Hurt(collisionDirection, 3);

                // Show damage vignette when player health low
                if (GetHealth() == 1)
                {
                    CameraSystem.cameraSystem.FadeInVignette(Color.black, 2);
                    StartCoroutine(heartbeat);
                }

                // Die if player health is zero
                if (HealthZero())
                {
                    Die();
                }
                else
                {
                    animator.Play(hurt.GetHash());
                }
                break;
            default:
                // Unhandled collision tag
                break;
        }
    }


    /**************************
     *        Attack          *
     **************************/
    public void UpdateAttackChain()
    {
        // Keep attack chain time for a few frames after stopping attack
        if (attackChainWaitFrames > 0)
        {
            attackChainWaitFrames--;
        }
        else
        {
            animator.SetInteger("AttackChain", 0);
        }
    }

    private bool InterruptAttack()
    {
        if (currentStateInfo.fullPathHash == attack1.GetHash() && attack1.Interrupt(animator))
        {
            return true;
        }
        else if (currentStateInfo.fullPathHash == attack2.GetHash() && attack2.Interrupt(animator))
        {
            return true;
        }
        else if (currentStateInfo.fullPathHash == attack3.GetHash() && attack3.Interrupt(animator))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsInAttackStateAndInvulnerable()
    {
        if (currentStateInfo.fullPathHash == attack1.GetHash())
        {
            if (!attack1.IsVulnerable())
            {
                return true;
            }
        }
        else if (currentStateInfo.fullPathHash == attack2.GetHash())
        {
            if (!attack2.IsVulnerable())
            {
                return true;
            }
        }
        else if (currentStateInfo.fullPathHash == attack3.GetHash())
        {
            if (!attack3.IsVulnerable())
            {
                return true;
            }
        }
        else if (currentStateInfo.fullPathHash == chargedAttack.GetHash())
        {
            if (!chargedAttack.IsVulnerable())
            {
                return true;
            }
        }

        return false;
    }


    /**************************
     *          Hurt          *
     **************************/
    private new void Hurt(Vector2 collisionDirection, float force)
    {
        base.Hurt(collisionDirection, force);
        DecreaseHealth(1);
    }

    private IEnumerator NearDeathHeartbeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f);
            PlaySfx(9, 1, 1);
        }
    }


    /**************************
     *         Death          *
     **************************/
    public void BeginDeath()
    {
        // Interrupt dash if currently dashing
        if (currentStateInfo.fullPathHash == dash.GetHash())
        {
            dash.Interrupt(animator);
        }

        StopCoroutine(heartbeat);

        animator.SetBool("Attacking", false);
        animator.SetBool("Idling", false);
        animator.SetBool("Ranged", false);
    }

    public void Die()
    {
        BeginDeath();
        animator.SetBool("Dead", true);

        IgnoreAllPhysics();
        SetSortingLayer("UI");
        CameraSystem.cameraSystem.FadeOutToBlack(2);

        animator.Play("PlayerDie");
    }

    public void Fall()
    {
        BeginDeath();

        gameObject.layer = 10;
        SetSortingLayer("UI");
        CameraSystem.cameraSystem.FadeOutToBlack(0.75f);

        SetInput(false);
        SetSpeed(40);
        SetNextVelocity(facingDirection.normalized);

        animator.Play("PlayerFallStart");
    }

    public void Revive()
    {
        // Restore defaults on everything
        CameraSystem.cameraSystem.FadeOutVignette(0);
        animator.SetBool("Dead", false);
        RestoreFullHealth();
        FadeInSprite(0);
        SetSortingLayer("Entity");
        falling = false;
        RestoreDefaultSpeed();
        LevelSystem.levelSystem.ReviveTransition();
    }

    public bool PlayerIsFalling()
    {
        return falling;
    }


    /**************************
     *       Transition       *
     **************************/
    public void Transition(Vector2 newPosition)
    {
        // Interrupt dash if currently dashing
        if (currentStateInfo.fullPathHash == dash.GetHash())
        {
            dash.Interrupt(animator);
        }
        // Disable all collisions
        gameObject.layer = 10;
        // Set new position
        body.position = newPosition;
    }

    public void EndTransition()
    {
        // Enable collisions
        gameObject.layer = 12;
    }


    /**************************
     *         Input          *
     **************************/
    private Vector2 GetAxisInput()
    {
        return new Vector2(Input.GetAxisRaw("LeftAxisX"), Input.GetAxisRaw("LeftAxisY"));
    }

    public void SetInput(bool setting)
    {
        inputEnabled = setting;
    }
}
