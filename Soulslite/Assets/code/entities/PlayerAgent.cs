using UnityEngine;
using System.Collections;


public class PlayerAgent : BaseEntity
{
    private DashTrail dashTrail;
    private bool attacking = false;
    private bool dashing = false;

    private AnimatorStateInfo currentStateInfo;
    private DashStateMachine dashStateMachine;
    private int dashStateHash = Animator.StringToHash("Base Layer.PlayerDashState");
    private int attackStateHash = Animator.StringToHash("Base Layer.PlayerAttackState");


    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();
        dashStateMachine = animator.GetBehaviour<DashStateMachine>();
        dashStateMachine.Setup(GetComponent<DashTrail>(), this);
    }


    /**************************
     *        Update          *
     **************************/
    private new void Update()
    {
        base.Update();

        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        /*****************
         * DEFAULT
         *****************/
        if (currentStateInfo.fullPathHash != dashStateHash && 
            currentStateInfo.fullPathHash != attackStateHash)
        {
            speedMultiplier = 60f;
            nextVelocity.x = Input.GetAxis("LeftAxisX") * speedMultiplier;
            nextVelocity.y = Input.GetAxis("LeftAxisY") * speedMultiplier;

            // Dash input handling
            if (Input.GetButtonDown("Button0"))
            {
                animator.SetTrigger("Dash");
            }

            if (Input.GetButtonDown("Button1"))
            {
                animator.SetTrigger("Attack");
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
        //if (dashing && collision.gameObject.tag == "EnvironmentObstacle")
        //{
        //    PlayerHaltDash();
        //    PlayerEndDash();
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(Hurt(collision.attachedRigidbody.velocity));
    }


    /**************************
     *          Hurt          *
     **************************/
    private IEnumerator Hurt(Vector2 collisionVelocity)
    {
        canMove = false;
        animator.SetBool("Hurt", true);

        if (currentStateInfo.fullPathHash == dashStateHash)
        {
            dashStateMachine.Interrupt(animator);
        }

        if (currentStateInfo.fullPathHash == attackStateHash)
        {
            PlayerAttackEnd();
        }

        animator.Play("PlayerHurtState");

        spriteRenderer.material.SetFloat("_FlashAmount", 0.85f);
        yield return new WaitForSeconds(0.025f);
        spriteRenderer.material.SetFloat("_FlashAmount", 0f);
    }

    private void EndHurt()
    {
        canMove = true;
        animator.SetBool("Hurt", false);
    }


    /**************************
     *        Attack          *
     **************************/
    private void PlayerAttackStart()
    {
        speedMultiplier = 60f;
        nextVelocity = facingDirection * speedMultiplier;
    }

    private void PlayerAttackPost()
    {
        nextVelocity = Vector2.zero;
    }

    private void PlayerAttackEnd()
    {
        attacking = false;
    }
}
