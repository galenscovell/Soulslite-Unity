﻿using UnityEngine;


public class PlayerController : MonoBehaviour 
{
    private Animator animator;
    private Rigidbody2D body;

    private const float epsilon = 0.2f;
    private float speedLimit;
    private bool moving = false;
    private bool dashing = false;

    private Vector2 previousDirection = new Vector2(0, 0);
    private Vector2 zeroVector = new Vector2(0, 0);



   /**************************
    *          Init          *
    **************************/
    void Awake() 
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

	void Start() 
    {
        
    }



   /**************************
    *        Update          *
    **************************/
    void FixedUpdate() 
    {
        /***********
         * Default *
         */
        if (!dashing) 
        {
            speedLimit = 60f;

            float newX = Input.GetAxis("LeftAxisX") * speedLimit;
            float newY = Input.GetAxis("LeftAxisY") * speedLimit;

            // Dash input handling
            if (Input.GetButtonDown("Button0"))
            {
                // If not moving, set start dash velocity in facing direction
                if (!IsMoving())
                {
                    newX = previousDirection.x * speedLimit;
                    newY = previousDirection.y * speedLimit;
                }
                dashing = true;
            }
            
            body.velocity = NormalizedDiagonal(newX, newY);

            // Determine last direction faced
            moving = IsMoving();
            if (moving)
            {
                SetDirection();
            }
        }
        /***********
         * Dashing *
         */
        else if (dashing)
        {
            speedLimit = 1200f;
        }
        
        // Update animator parameters
        animator.SetBool("Dashing", dashing);
        animator.SetBool("Moving", moving);
	}



    /**************************
     *          Util          *
     **************************/
    /// <summary>
    /// Find currently facing direction and set animator direction parameters.
    /// </summary>
    private void SetDirection()
    {
        if (Mathf.Abs(body.velocity.x) > Mathf.Abs(body.velocity.y))
        {
            if (body.velocity.x > 0)
            {
                previousDirection.x = 1;   // Right
                previousDirection.y = 0;
            }
            else
            {
                previousDirection.x = -1;  // Left
                previousDirection.y = 0;
            }
        }
        else if (Mathf.Abs(body.velocity.x) < Mathf.Abs(body.velocity.y))
        {
            if (body.velocity.y > 0)
            {
                previousDirection.x = 0;   // Up
                previousDirection.y = 1;
            }
            else
            {
                previousDirection.x = 0;   // Down
                previousDirection.y = -1;
            }
        }

        animator.SetFloat("DirectionX", previousDirection.x);
        animator.SetFloat("DirectionY", previousDirection.y);
    }

    /// <summary>
    /// Take in a velocity and normalize it in the diagonal direction.
    /// </summary>
    /// <param name="velocityX"></param>
    /// <param name="velocityY"></param>
    /// <returns>Normalized velocity vector2</returns>
    private Vector2 NormalizedDiagonal(float velocityX, float velocityY)
    {
        float pythagoras = (velocityX * velocityX) + (velocityY * velocityY);
        if (pythagoras > (speedLimit * speedLimit))
        {
            float magnitude = Mathf.Sqrt(pythagoras);
            float multiplier = speedLimit / magnitude;
            velocityX *= multiplier;
            velocityY *= multiplier;
        }
        return new Vector2(velocityX, velocityY);
    }

    /// <summary>
    /// Return whether the current player body velocity is considered 'moving'.
    /// </summary>
    /// <returns></returns>
    private bool IsMoving() 
    {
        return Vector2.Distance(body.velocity, zeroVector) > epsilon;
    }



   /**************************
    *          Dash          *
    **************************/
    /// <summary>
    /// Dash animation event -- begin dashing.
    /// </summary>
    private void StartDash()
    {
        float dashX = body.velocity.x * speedLimit;
        float dashY = body.velocity.y * speedLimit;
        body.velocity = NormalizedDiagonal(dashX, dashY);
    }

    /// <summary>
    /// Dash animation event -- stop forward motion of dash.
    /// </summary>
    private void HaltDash()
    {
        body.velocity = new Vector2(0, 0);
    }

    /// <summary>
    /// Dash animation event -- end dashing.
    /// </summary>
    private void EndDash()
    {
        dashing = false;
    }
}