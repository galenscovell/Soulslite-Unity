using UnityEngine;


public class PlayerAgent : BaseEntity
{
    private AnimatorStateInfo currentStateInfo;

    private PlayerAttackOneState attackOneState;
    private int attackOneStateHash = Animator.StringToHash("Base Layer.PlayerAttackOneState");
    private PlayerAttackTwoState attackTwoState;
    private int attackTwoStateHash = Animator.StringToHash("Base Layer.PlayerAttackTwoState");

    private PlayerDashState dashState;
    private int dashStateHash = Animator.StringToHash("Base Layer.PlayerDashState");

    private PlayerHurtState hurtState;
    private int hurtStateHash = Animator.StringToHash("Base Layer.PlayerHurtState");

    private PlayerMovementState movementState;
    private int movementStateHash = Animator.StringToHash("Base Layer.PlayerMovementState");

    private CameraShaker cameraShaker;


    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();

        AudioSource attackOneSound = soundEffects[0];
        AudioSource attackTwoSound = soundEffects[1];
        AudioSource dashSound = soundEffects[2];
        AudioSource footstepSound = soundEffects[3];
        AudioSource hurtSound = soundEffects[4];

        attackOneState = animator.GetBehaviour<PlayerAttackOneState>();
        attackOneState.Setup(this, attackOneSound);
        attackTwoState = animator.GetBehaviour<PlayerAttackTwoState>();
        attackTwoState.Setup(this, attackTwoSound);

        dashState = animator.GetBehaviour<PlayerDashState>();
        dashState.Setup(this, GetComponent<DashTrail>(), dashSound);

        hurtState = animator.GetBehaviour<PlayerHurtState>();
        hurtState.Setup(this, hurtSound);

        movementState = animator.GetBehaviour<PlayerMovementState>();
        movementState.Setup(this, footstepSound);

        cameraShaker = GetComponent<CameraShaker>();
    }


    /**************************
     *        Update          *
     **************************/
    private new void Update()
    {
        base.Update();

        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (currentStateInfo.fullPathHash != attackOneStateHash &&
            currentStateInfo.fullPathHash != attackTwoStateHash &&
            currentStateInfo.fullPathHash != dashStateHash &&
            currentStateInfo.fullPathHash != hurtStateHash)
        {
            SetSpeed(GetNormalSpeed());
            SetNextVelocity(new Vector2(
                Input.GetAxis("LeftAxisX") * speedMultiplier,
                Input.GetAxis("LeftAxisY") * speedMultiplier
            ));

            if (Input.GetButtonDown("Button0"))
            {
                animator.SetBool("Attacking", true);
            }

            if (Input.GetButtonDown("Button1"))
            {
                animator.SetBool("Dashing", true);
            }
        }
        else if (currentStateInfo.fullPathHash == dashStateHash)
        {
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

            if (currentStateInfo.fullPathHash == dashStateHash)
            {
                dashState.Interrupt(animator);
            }

            Hurt();
        }
    }


    /**************************
     *          Hurt          *
     **************************/
    private new void Hurt()
    {
        base.Hurt();
        cameraShaker.Activate();

        animator.Play("PlayerHurtState");
    }
}
