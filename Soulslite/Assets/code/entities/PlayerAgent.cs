using System.Collections;
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

    private CameraShaker cameraShaker;


    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();

        attackState = animator.GetBehaviour<PlayerAttackState>();
        attackState.Setup(this);

        dashState = animator.GetBehaviour<PlayerDashState>();
        dashState.Setup(this, GetComponent<DashTrail>());

        hurtState = animator.GetBehaviour<PlayerHurtState>();
        hurtState.Setup(this);

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
        animator.Play("PlayerHurtState");
        cameraShaker.Activate();

        base.Hurt();
    }
}
