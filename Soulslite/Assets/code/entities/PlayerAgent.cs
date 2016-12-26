using UnityEngine;
using System.Collections;


public class PlayerAgent : BaseEntity
{
    private DashTrail dashTrail;
    private bool dashing = false;


    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();
        dashTrail = GetComponent<DashTrail>();
    }


    /**************************
     *        Update          *
     **************************/
    private new void Update()
    {
        base.Update();

        /*****************
         * DEFAULT
         *****************/
        if (!dashing)
        {
            speedMultiplier = 60f;
            nextVelocity.x = Input.GetAxis("LeftAxisX") * speedMultiplier;
            nextVelocity.y = Input.GetAxis("LeftAxisY") * speedMultiplier;

            // Dash input handling
            if (Input.GetButtonDown("Button0"))
            {
                dashing = true;
            }
        }

        animator.SetBool("Dashing", dashing);
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
        //if (dashing)
        //{
        //    canMove = false;
        //    animator.SetBool("Hurt", true);

        //    PlayerInterruptDash();
        //    PlayerHaltDash();
        //    PlayerEndDash();
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.tag);
        StartCoroutine(Hurt(collision.attachedRigidbody.velocity));
    }


    /**************************
     *          Hurt          *
     **************************/
    private IEnumerator Hurt(Vector2 collisionVelocity)
    {
        canMove = false;
        animator.SetBool("Hurt", true);

        if (dashing)
        {
            PlayerInterruptDash();
            PlayerHaltDash();
            PlayerEndDash();
        } 
        else
        {
            animator.Play("PlayerHurtState");
        }

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
     *          Dash          *
     **************************/
    /// <summary>
    /// Dash animation event -- begin dash
    /// </summary>
    private void PlayerStartDash()
    {
        dashTrail.SetEnabled(true);
        speedMultiplier = 1200f;
        nextVelocity = facingDirection * speedMultiplier;
    }

    /// <summary>
    /// Dash animation event -- stop forward motion of dash
    /// </summary>
    private void PlayerHaltDash()
    {
        dashTrail.SetEnabled(false);
        nextVelocity = Vector2.zero;
    }

    /// <summary>
    /// Dash animation event -- end dash
    /// </summary>
    private void PlayerEndDash()
    {
        dashing = false;
    }

    /// <summary>
    /// Dash animation event -- dash abruptly stopped (ie from wall or enemy attack)
    /// </summary>
    private void PlayerInterruptDash()
    {
        animator.Play("PlayerDashCrashState");
    }
}
