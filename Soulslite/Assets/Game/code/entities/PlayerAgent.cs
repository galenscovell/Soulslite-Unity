using UnityEngine;


public class PlayerAgent : BaseEntity
{
    private AnimatorStateInfo currentStateInfo;

    private PlayerAttackOneState attackOneState;
    private PlayerAttackTwoState attackTwoState;
    private PlayerAttackThreeState attackThreeState;
    private PlayerDashState dashState;
    private PlayerHurtState hurtState;
    private PlayerMovementState movementState;

    private int attackOneStateHash = Animator.StringToHash("Base Layer.PlayerAttackOneState");
    private int attackTwoStateHash = Animator.StringToHash("Base Layer.PlayerAttackTwoState");
    private int attackThreeStateHash = Animator.StringToHash("Base Layer.PlayerAttackThreeState");
    private int dashStateHash = Animator.StringToHash("Base Layer.PlayerDashState");
    private int hurtStateHash = Animator.StringToHash("Base Layer.PlayerHurtState");
    private int idleStateHash = Animator.StringToHash("Base Layer.PlayerIdleState");
    private int movementStateHash = Animator.StringToHash("Base Layer.PlayerMovementState");
    private int rangeStateHash = Animator.StringToHash("Base Layer.PlayerRangeState");

    private CameraShaker cameraShaker;
    private int attackChainWaitFrames;


    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();

        attackOneState = animator.GetBehaviour<PlayerAttackOneState>();
        attackTwoState = animator.GetBehaviour<PlayerAttackTwoState>();
        attackThreeState = animator.GetBehaviour<PlayerAttackThreeState>();
        dashState = animator.GetBehaviour<PlayerDashState>();
        movementState = animator.GetBehaviour<PlayerMovementState>();
        hurtState = animator.GetBehaviour<PlayerHurtState>();

        // Ints in state Setups are sfx index
        attackOneState.Setup(this, 0);
        attackTwoState.Setup(this, 1);
        attackThreeState.Setup(this, 1);
        dashState.Setup(this, GetComponent<DashTrail>(), 2);
        movementState.Setup(this, 3);
        hurtState.Setup(this, 4);

        cameraShaker = GetComponent<CameraShaker>();
    }


    /**************************
     *        Update          *
     **************************/
    private new void Update()
    {
        base.Update();

        // Keep attack chain time for a few frames after stopping attack
        if (attackChainWaitFrames > 0)
        {
            attackChainWaitFrames--;
        }
        else
        {
            ResetAttackChain();
        }

        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        /****************
         * MOVE OR IDLE
         ****************/
        if (currentStateInfo.fullPathHash == movementStateHash || currentStateInfo.fullPathHash == idleStateHash)
        {
            SetSpeed(GetDefaultSpeed());
            SetNextVelocity(new Vector2(
                Input.GetAxis("LeftAxisX") * speed,
                Input.GetAxis("LeftAxisY") * speed
            ));

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
            // Start Dash
            else if (Input.GetButtonDown("Button1"))
            {
                animator.SetBool("Dashing", true);
            }
            else if (Input.GetButtonDown("Button7"))
            {
                animator.SetBool("Ranged", true);
            }
        }
        /****************
         * DASHING
         ****************/
        else if (currentStateInfo.fullPathHash == dashStateHash)
        {
            // Chain dash
            if (Input.GetButtonDown("Button1"))
            {
                Vector2 dashDirection = new Vector2(
                    Input.GetAxis("LeftAxisX"), Input.GetAxis("LeftAxisY")
                );

                if (dashDirection.magnitude == 0)
                {
                    dashDirection = facingDirection;
                }

                dashState.Chain(animator, dashDirection);
            }
        }
        /****************
         * RANGED
         ****************/
        else if (currentStateInfo.fullPathHash == rangeStateHash)
        {
            SetNextVelocity(Vector2.zero);

            Vector2 direction = new Vector2(
                Input.GetAxis("LeftAxisX"),
                Input.GetAxis("LeftAxisY")
            );

            // Reduce precision of axis for more forgiving ranged aiming
            if (direction.magnitude > 0.7f)
            {
                SetFacingDirection(direction);
            }

            if (!Input.GetButton("Button7"))
            {
                animator.SetBool("Ranged", false);
            }
        }
    }

    private new void FixedUpdate()
    {
        // Will update body velocity and facing direction
        base.FixedUpdate();
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
            if (currentStateInfo.fullPathHash == hurtStateHash)
            {
                return;
            }

            if (currentStateInfo.fullPathHash == attackOneStateHash && !attackOneState.Interrupt(animator))
            {
                return;
            }

            if (currentStateInfo.fullPathHash == attackTwoStateHash && !attackTwoState.Interrupt(animator))
            {
                return;
            }

            if (currentStateInfo.fullPathHash == attackThreeStateHash && !attackThreeState.Interrupt(animator))
            {
                return;
            }

            if (currentStateInfo.fullPathHash == dashStateHash)
            {
                dashState.Interrupt(animator);
            }

            Vector2 collisionDirection = transform.position - collision.transform.position;
            Hurt(collisionDirection);
        }
    }


    /**************************
     *        Attack          *
     **************************/
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
        cameraShaker.Activate();

        animator.Play("PlayerHurtState");
    }
}
