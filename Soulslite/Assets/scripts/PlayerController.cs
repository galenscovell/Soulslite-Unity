using UnityEngine;


public class PlayerController : MonoBehaviour {
    private const float speed = 60f;
    private Rigidbody2D body;
    private Animator animator;

    private bool inMotion = false;
    private Vector2 lastMove = new Vector2(0, 0);
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
        float newX = Input.GetAxis("LeftAxisX") * speed;
        float newY = Input.GetAxis("LeftAxisY") * speed;

        // Normalize diagonal movement
        float pythagoras = (newX * newX) + (newY * newY);
        if (pythagoras > (speed * speed)) {
            float magnitude = Mathf.Sqrt(pythagoras);
            float multiplier = speed / magnitude;
            newX *= multiplier;
            newY *= multiplier;
        }

        // Set new velocity
        body.velocity = new Vector2(newX, newY);

        // Update animator
        if (Vector2.Distance(body.velocity, zeroVector) > 0.2f) {
            inMotion = true;
            lastMove.x = newX;
            lastMove.y = newY;
        }
        
        animator.SetFloat("MoveX", body.velocity.x);
        animator.SetFloat("MoveY", body.velocity.y);
        animator.SetFloat("LastMoveX", lastMove.x);
        animator.SetFloat("LastMoveY", lastMove.x);
        animator.SetBool("InMotion", inMotion);
	}
}
