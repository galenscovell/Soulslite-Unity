using UnityEngine;
using System.Collections;


public class PlayerAgent : BaseEntity
{
    private AnimatorStateInfo currentStateInfo;

    private PlayerAttackState attackState;
    private int attackStateHash = Animator.StringToHash("Base Layer.PlayerAttackState");

    private PlayerDashState dashState;
    private int dashStateHash = Animator.StringToHash("Base Layer.PlayerDashState");

    private PlayerHurtState hurtState;
    private int hurtStateHash = Animator.StringToHash("Base Layer.PlayerHurtState");


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
            speedMultiplier = 60f;
            nextVelocity.x = Input.GetAxis("LeftAxisX") * speedMultiplier;
            nextVelocity.y = Input.GetAxis("LeftAxisY") * speedMultiplier;

            if (Input.GetButtonDown("Button0")) animator.SetBool("Dashing", true);
            if (Input.GetButtonDown("Button1")) animator.SetTrigger("Attack");
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
        StartCoroutine(Hurt());
    }


    /**************************
     *          Hurt          *
     **************************/
    private IEnumerator Hurt()
    {
        if (currentStateInfo.fullPathHash == attackStateHash) attackState.Interrupt(animator);
        if (currentStateInfo.fullPathHash == dashStateHash) dashState.Interrupt(animator);

        animator.Play("PlayerHurtState");

        spriteRenderer.material.SetFloat("_FlashAmount", 0.85f);
        yield return new WaitForSeconds(0.025f);
        spriteRenderer.material.SetFloat("_FlashAmount", 0f);
    }
}
