using UnityEngine;


public class PlayerController : MonoBehaviour 
{
    private Animator animator;
    private Rigidbody2D body;

    private const float walkSpeed = 60f;
    private const float dashSpeed = 600f;
    private const float epsilon = 0.2f;
    private bool inMotion = false;
    private bool dashing = false;

    private Vector2 previousDirection = new Vector2(0, 0);
    private Vector2 zeroVector = new Vector2(0, 0);


    void Awake() 
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

	void Start() 
    {
        
    }

	void FixedUpdate() 
    {
        inMotion = false;
        float newX = 0;
        float newY = 0;
        float speedLimit = walkSpeed;
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        /***********
         * Default *
         */
        if (currentState.IsTag("DefaultState")) 
        {
            newX = Input.GetAxis("LeftAxisX") * walkSpeed;
            newY = Input.GetAxis("LeftAxisY") * walkSpeed;

            // Dash input handling
            if (Input.GetButtonDown("Button0")) dashing = true;
        }
        /***********
         * Dashing *
         */
        else if (currentState.IsTag("DashState"))
        {
            speedLimit = dashSpeed;

            newX = previousDirection.x * dashSpeed;
            newY = previousDirection.y * dashSpeed;
            print(newX);
            print(newY);

            if (currentState.normalizedTime > 0.625f)
            {
                dashing = false;
            }
        }

        // Normalize diagonal movement
        float pythagoras = (newX * newX) + (newY * newY);
        if (pythagoras > (speedLimit * speedLimit)) 
        {
            float magnitude = Mathf.Sqrt(pythagoras);
            float multiplier = speedLimit / magnitude;
            newX *= multiplier;
            newY *= multiplier;
        }

        // Set new velocity
        body.velocity = new Vector2(newX, newY);

        // Update animator
        if (InMotion()) inMotion = true;

        // Determine last direction faced
        if (inMotion) 
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
        }

        animator.SetFloat("DirectionX", previousDirection.x);
        animator.SetFloat("DirectionY", previousDirection.y);
        animator.SetBool("Dashing", dashing);
        animator.SetBool("InMotion", inMotion);
	}

    private bool InMotion() 
    {
        return Vector2.Distance(body.velocity, zeroVector) > epsilon;
    }
}
