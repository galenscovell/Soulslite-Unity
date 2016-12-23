﻿using UnityEngine;


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

        // Update animator parameters
        animator.SetBool("Dashing", dashing);
    }

    private new void FixedUpdate() 
    {
        base.FixedUpdate();
	}



    /**************************
     *      Collisions        *
     **************************/
    private new void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }



   /**************************
    *          Dash          *
    **************************/
    /// <summary>
    /// Dash animation event -- begin dashing.
    /// </summary>
    private void StartDash()
    {
        speedMultiplier = 800f;
        nextVelocity.x = direction.x * speedMultiplier;
        nextVelocity.y = direction.y * speedMultiplier;
        body.velocity = NormalizedDiagonal(nextVelocity);
        dashTrail.SetEnabled(true);
    }

    /// <summary>
    /// Dash animation event -- stop forward motion of dash.
    /// </summary>
    private void HaltDash()
    {
        nextVelocity = Vector2.zero;
        body.velocity = nextVelocity;
        dashTrail.SetEnabled(false);
    }

    /// <summary>
    /// Dash animation event -- end dashing.
    /// </summary>
    private void EndDash()
    {
        dashing = false;
    }
}
