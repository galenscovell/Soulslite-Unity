using UnityEngine;


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
        dashTrail.SetEnabled(true);

        speedMultiplier = 800f;
        nextVelocity = facingDirection * speedMultiplier;
    }

    /// <summary>
    /// Dash animation event -- stop forward motion of dash.
    /// </summary>
    private void HaltDash()
    {
        dashTrail.SetEnabled(false);

        nextVelocity = Vector2.zero;
    }

    /// <summary>
    /// Dash animation event -- end dashing.
    /// </summary>
    private void EndDash()
    {
        dashing = false;
    }
}
