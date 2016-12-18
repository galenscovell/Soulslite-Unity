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
    }

    private new void FixedUpdate() 
    {
        if (!dashing) 
        {
            speedMultiplier = 60f;
            float newX = Input.GetAxis("LeftAxisX") * speedMultiplier;
            float newY = Input.GetAxis("LeftAxisY") * speedMultiplier;

            // Dash input handling
            if (Input.GetButtonDown("Button0"))
            {
                dashing = true;

                // If not moving, set start dash velocity in facing direction
                if (!IsMoving())
                {
                    newX = previousDirection.x * speedMultiplier;
                    newY = previousDirection.y * speedMultiplier;
                }
            }
            
            // Set new velocity
            body.velocity = NormalizedDiagonal(newX, newY);
        }
        
        // Update animator parameters
        animator.SetBool("Dashing", dashing);

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
        speedMultiplier = 2400f;
        float dashX = body.velocity.x * speedMultiplier;
        float dashY = body.velocity.y * speedMultiplier;
        body.velocity = NormalizedDiagonal(dashX, dashY);
        dashTrail.SetEnabled(true);
    }

    /// <summary>
    /// Dash animation event -- stop forward motion of dash.
    /// </summary>
    private void HaltDash()
    {
        body.velocity = new Vector2(0, 0);
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
