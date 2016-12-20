using UnityEngine;


public class BaseEntity : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D body;
    protected SpriteRenderer spriteRenderer;

    protected float speedMultiplier;
    protected bool moving = false;

    protected Vector2 direction;
    protected Vector2 nextVelocity = new Vector2(0, 0);



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
    protected void UpdateSortingLayer()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt((transform.position.y + -0.1f) / 0.05f);
    }

    protected void Update()
    {
        UpdateSortingLayer();
    }

    protected void FixedUpdate()
    {
        // Determine last direction faced
        moving = IsMoving();
        if (moving) SetDirection();

        // Update animator
        animator.SetBool("Moving", moving);
    }



    /**************************
     *      Collisions        *
     **************************/
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        print("Collision: " + gameObject.name + ", " + collision.collider.name);
    }


    /**************************
     *          Util          *
     **************************/
    /// <summary>
    /// Set animator direction as normalized current velocity.
    /// </summary>
    protected void SetDirection()
    {
        Vector2 direction = body.velocity.normalized;
        animator.SetFloat("DirectionX", direction.x);
        animator.SetFloat("DirectionY", direction.y);
    }

    /// <summary>
    /// Take in a velocity and normalize it in the diagonal direction.
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>Normalized velocity vector2</returns>
    protected Vector2 NormalizedDiagonal(Vector2 velocity)
    {
        float pythagoras = (velocity.x * velocity.x) + (velocity.y * velocity.y);
        if (pythagoras > (speedMultiplier * speedMultiplier))
        {
            float magnitude = Mathf.Sqrt(pythagoras);
            float multiplier = speedMultiplier / magnitude;
            velocity.x *= multiplier;
            velocity.y *= multiplier;
        }
        return new Vector2(velocity.x, velocity.y);
    }

    /// <summary>
    /// Return whether the current body velocity is considered 'moving'.
    /// </summary>
    /// <returns>true if velocity is significant</returns>
    protected bool IsMoving()
    {
        return body.velocity.magnitude > 0.2f;
    }
}
