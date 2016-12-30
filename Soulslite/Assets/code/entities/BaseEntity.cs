using UnityEngine;


public class BaseEntity : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D body;
    protected SpriteRenderer spriteRenderer;

    public Vector2 facingDirection = Vector2.zero;
    public Vector2 nextVelocity = Vector2.zero;
    public float speedMultiplier;
    public bool canMove = true;
    

    /**************************
     *          Init          *
     **************************/
    protected void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    /**************************
     *        Update          *
     **************************/
    protected void Update()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt((transform.position.y + -0.1f) / 0.05f);
    }

    protected void FixedUpdate()
    {
        // Entity is allowed to move
        if (canMove)
        {
            // If moving, set direction moved in
            if (IsMoving())
            {
                SetFacingDirection();
                animator.SetBool("Moving", true);
            }
            // If idle, use last direction moved in as facing direction
            else
            {
                animator.SetBool("Moving", false);
            }
        }
        // Entity is not allowed to move
        else
        {
            nextVelocity = Vector2.zero;
        }

        // Set new body velocity based on updated nextVelocity
        body.velocity = nextVelocity.normalized * speedMultiplier;
    }


    /**************************
     *          Util          *
     **************************/
    /// <summary>
    /// Set facing direction as normalized current velocity.
    /// </summary>
    protected void SetFacingDirection()
    {
        facingDirection = nextVelocity.normalized;
        animator.SetFloat("DirectionX", facingDirection.x);
        animator.SetFloat("DirectionY", facingDirection.y);
    }

    /// <summary>
    /// Return whether the current body velocity is considered 'moving'.
    /// </summary>
    /// <returns>true if velocity is significant</returns>
    protected bool IsMoving()
    {
        return Vector2.Distance(nextVelocity, Vector2.zero) > 0.2f;
    }
}
