using UnityEngine;


public class PlayerAgent : BaseEntity
{
    private AnimatorStateInfo currentStateInfo;
    private Transform playerGunBarrel;
    private int attackChainWaitFrames;

    // State machines
    private PlayerAttack1 attack1;
    private PlayerAttack2 attack2;
    private PlayerAttack3 attack3;
    private PlayerDash dash;
    private PlayerHurt hurt;
    private PlayerIdle idle;
    private PlayerMovement movement;
    private PlayerRangedStart rangedStart;
    private PlayerRangedReady rangedReady;
    private PlayerRangedShot rangedShot;
    private PlayerRangedExit rangedExit;


    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();

        playerGunBarrel = transform.Find("playerGun").gameObject.transform;

        attack1 = animator.GetBehaviour<PlayerAttack1>();
        attack2 = animator.GetBehaviour<PlayerAttack2>();
        attack3 = animator.GetBehaviour<PlayerAttack3>();
        dash = animator.GetBehaviour<PlayerDash>();
        hurt = animator.GetBehaviour<PlayerHurt>();
        idle = animator.GetBehaviour<PlayerIdle>();
        movement = animator.GetBehaviour<PlayerMovement>();
        rangedStart = animator.GetBehaviour<PlayerRangedStart>();
        rangedReady = animator.GetBehaviour<PlayerRangedReady>();
        rangedShot = animator.GetBehaviour<PlayerRangedShot>();
        rangedExit = animator.GetBehaviour<PlayerRangedExit>();

        // Ints in state Setups are sfx index
        attack1.Setup(this, 0);
        attack2.Setup(this, 1);
        attack3.Setup(this, 1);
        dash.Setup(this, GetComponent<DashTrail>(), 2);
        hurt.Setup(this, 3);
        idle.Setup(this);
        movement.Setup(this, 4);
        rangedStart.Setup(this, 5);
        rangedReady.Setup(this, GetComponent<LineRenderer>());
        rangedShot.Setup(this, 6);
        rangedExit.Setup(this);
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
        else if (currentStateInfo.fullPathHash == rangedStart.GetHash() ||
            currentStateInfo.fullPathHash == rangedReady.GetHash() ||
            currentStateInfo.fullPathHash == rangedExit.GetHash())
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
        // Prevent actual movement, but allow rotation for direction control
        SetNextVelocity(Vector2.zero);
        Vector2 direction = GetAxisInput();

        // Reduce precision of axis input
        if (direction.magnitude > 0.8f)
        {
            SetFacingDirection(direction);
        }

        // Limit facing direction to 8-way only
        SnapFacingDirection8Way();

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

    public Vector3 GetPlayerGunBarrel()
    {
        return playerGunBarrel.position;
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
     *         Input          *
     **************************/
    private Vector2 GetAxisInput()
    {
        return new Vector2(Input.GetAxis("LeftAxisX"), Input.GetAxis("LeftAxisY"));
    }
}
