using UnityEngine;


public class PlayerAgent : BaseEntity
{
    private AnimatorStateInfo currentStateInfo;

    private PlayerAttackState attackState;
    private int attackStateHash = Animator.StringToHash("Base Layer.PlayerAttackState1");

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

        AudioSource attackSound = soundEffects[0];
        AudioSource dashSound = soundEffects[1];
        AudioSource footstepSound = soundEffects[2];

        attackState = animator.GetBehaviour<PlayerAttackState>();
        attackState.Setup(this, attackSound);

        dashState = animator.GetBehaviour<PlayerDashState>();
        dashState.Setup(this, GetComponent<DashTrail>(), dashSound);

        hurtState = animator.GetBehaviour<PlayerHurtState>();
        hurtState.Setup(this);

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

        if (currentStateInfo.fullPathHash != attackStateHash &&
            currentStateInfo.fullPathHash != dashStateHash &&
            currentStateInfo.fullPathHash != hurtStateHash)
        {
            SetSpeed(GetNormalSpeed());
            SetNextVelocity(new Vector2(
                Input.GetAxis("LeftAxisX") * speedMultiplier,
                Input.GetAxis("LeftAxisY") * speedMultiplier
            ));

            if (Input.GetButtonDown("Button0")) animator.SetBool("Dashing", true);
            if (Input.GetButtonDown("Button1")) animator.SetBool("Attacking", true);
        }
        else if (currentStateInfo.fullPathHash == dashStateHash)
        {
            if (Input.GetButtonDown("Button0"))
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

            if (currentStateInfo.fullPathHash == attackStateHash && !attackState.Interrupt(animator))
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
