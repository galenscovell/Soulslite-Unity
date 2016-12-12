using UnityEngine;


public class PlayerController : MonoBehaviour {
    private Rigidbody2D body;
    private Animator animator;

    private const float walkSpeed = 60f;
    private const float epsilon = 0.2f;
    private bool inMotion = false;
    private bool dashing = false;

    private Vector2 zeroVector = new Vector2(0, 0);


    void Awake() {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

	void Start() {
        
	}

	void FixedUpdate() {
        inMotion = false;

        // Establish new velocity
        float newX = Input.GetAxis("LeftAxisX") * walkSpeed;
        float newY = Input.GetAxis("LeftAxisY") * walkSpeed;

        // Normalize diagonal movement
        float pythagoras = (newX * newX) + (newY * newY);
        if (pythagoras > (walkSpeed * walkSpeed)) {
            float magnitude = Mathf.Sqrt(pythagoras);
            float multiplier = walkSpeed / magnitude;
            newX *= multiplier;
            newY *= multiplier;
        }

        // Set new velocity
        body.velocity = new Vector2(newX, newY);

        // Update animator
        if (InMotion()) {
            inMotion = true;
        }

        // Determine last direction faced
        if (inMotion) {
            if (Mathf.Abs(body.velocity.x) > Mathf.Abs(body.velocity.y)) {
                if (body.velocity.x > 0) {
                    animator.SetFloat("DirectionX", 1);   // Right
                    animator.SetFloat("DirectionY", 0);
                } else {
                    animator.SetFloat("DirectionX", -1);  // Left
                    animator.SetFloat("DirectionY", 0);
                }
            } else if (Mathf.Abs(body.velocity.x) < Mathf.Abs(body.velocity.y)) {
                if (body.velocity.y > 0) {
                    animator.SetFloat("DirectionY", 1);   // Up
                    animator.SetFloat("DirectionX", 0);
                } else {
                    animator.SetFloat("DirectionY", -1);  // Down
                    animator.SetFloat("DirectionX", 0);
                }
            }
        }

        animator.SetBool("InMotion", inMotion);
	}

    private bool InMotion() {
        return Vector2.Distance(body.velocity, zeroVector) > epsilon;
    }
}
