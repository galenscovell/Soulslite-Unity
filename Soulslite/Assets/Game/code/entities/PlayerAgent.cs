﻿using UnityEngine;


public class PlayerAgent : BaseEntity
{
    private AnimatorStateInfo currentStateInfo;
    private int attackChainWaitFrames;

    // State machines
    private PlayerAttack1 attack1;
    private PlayerAttack2 attack2;
    private PlayerAttack3 attack3;
    private PlayerDash dash;
    private PlayerHurt hurt;
    private PlayerIdle idle;
    private PlayerMovement movement;
    private PlayerRanged ranged;
    private PlayerRangedAttack rangedAttack;


    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();

        PlayerGunLimb playerGunLimb = transform.Find("playerGun").GetComponent<PlayerGunLimb>();

        attack1 = animator.GetBehaviour<PlayerAttack1>();
        attack2 = animator.GetBehaviour<PlayerAttack2>();
        attack3 = animator.GetBehaviour<PlayerAttack3>();
        dash = animator.GetBehaviour<PlayerDash>();
        hurt = animator.GetBehaviour<PlayerHurt>();
        idle = animator.GetBehaviour<PlayerIdle>();
        movement = animator.GetBehaviour<PlayerMovement>();
        ranged = animator.GetBehaviour<PlayerRanged>();
        rangedAttack = animator.GetBehaviour<PlayerRangedAttack>();

        // Ints in state Setups are sfx index
        attack1.Setup(this, 0);
        attack2.Setup(this, 1);
        attack3.Setup(this, 1);
        dash.Setup(this, GetComponent<DashTrail>(), 2);
        hurt.Setup(this, 3);
        idle.Setup(this);
        movement.Setup(this, 4);
        ranged.Setup(this, playerGunLimb, GetComponent<LineRenderer>(), 5);
        rangedAttack.Setup(this, playerGunLimb, 6);
    }


    /**************************
     *        Update          *
     **************************/
    private new void Update()
    {
        base.Update();
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
        SetSpeed(GetDefaultSpeed());
        SetNextVelocity(GetAxisInput() * speed);

        // Start attack
        if (Input.GetButtonDown("Button0"))
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
        else if (Input.GetButtonDown("Button1"))
        {
            animator.SetBool("Dashing", true);
        }
        // Start ranged attack mode
        else if (Input.GetButton("Button5"))
        {
            ranged.LoadGun();
            animator.SetBool("Ranged", true);
        }
    }

    private void DashUpdate()
    {
        // Chain dash
        if (Input.GetButtonDown("Button1"))
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
        Vector2 direction = GetAxisInput();

        // Reduce precision of axis input
        if (direction.magnitude > 0.4f)
        {
            SetFacingDirection(direction);
        }

        // Start ranged attack shot
        if (Input.GetButtonDown("Button0"))
        {
            animator.SetBool("Attacking", true);
        }

        // End ranged attack mode when button released
        if (!Input.GetButton("Button5"))
        {
            animator.SetBool("Attacking", false);
            animator.SetBool("Ranged", false);
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
        if (collision.tag != "Enemy" && collision.tag != "EnvironmentObstacle")
        {
            if (currentStateInfo.fullPathHash == hurt.GetHash())
            {
                return;
            }

            if (currentStateInfo.fullPathHash == attack1.GetHash() && !attack1.Interrupt(animator))
            {
                return;
            }

            if (currentStateInfo.fullPathHash == attack2.GetHash() && !attack2.Interrupt(animator))
            {
                return;
            }

            if (currentStateInfo.fullPathHash == attack3.GetHash() && !attack3.Interrupt(animator))
            {
                return;
            }

            if (currentStateInfo.fullPathHash == dash.GetHash())
            {
                dash.Interrupt(animator);
            }

            Vector2 collisionDirection = transform.position - collision.transform.position;
            Hurt(collisionDirection);
        }
    }


    /**************************
     *        Attack          *
     **************************/
    public void UpdateAttackChain()
    {
        // Keep attack chain time for a few frames after stopping attack
        if (attackChainWaitFrames > 0) attackChainWaitFrames--;
        else ResetAttackChain();
    }

    public void ResetAttackChain()
    {
        animator.SetInteger("AttackChain", 0);
    }

    public void ChainAttack()
    {
        animator.SetInteger("AttackChain", animator.GetInteger("AttackChain") + 1);
    }


    /**************************
     *          Hurt          *
     **************************/
    private new void Hurt(Vector2 collisionDirection)
    {
        base.Hurt(collisionDirection);
        CameraController.cameraController.ActivateShake();

        animator.Play(hurt.GetHash());
    }

    /**************************
     *         Death          *
     **************************/
    public void Die()
    {
        animator.SetBool("Attacking", false);
        animator.SetBool("Dashing", false);
        animator.SetBool("Idling", false);
        animator.SetBool("Ranged", false);
        EnableDeath();
    }


    /**************************
     *         Input          *
     **************************/
    private Vector2 GetAxisInput()
    {
        return new Vector2(Input.GetAxis("LeftAxisX"), Input.GetAxis("LeftAxisY"));
    }
}
