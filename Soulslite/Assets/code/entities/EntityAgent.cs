using UnityEngine;


public class EntityAgent : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;

    private float speedLimit;
    private bool moving = false;

    private Vector2 previousDirection = new Vector2(0, 0);
    private Vector2 zeroVector = new Vector2(0, 0);



    /**************************
     *          Init          *
     **************************/
    private void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }



    /**************************
     *        Update          *
     **************************/
    private void UpdateSortingLayer()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt((transform.position.y + -0.1f) / 0.05f);
    }

    private void Update()
    {
        UpdateSortingLayer();
    }

    private void FixedUpdate()
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("Collision: " + gameObject.name + ", " + collision.collider.name);
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
    /// Return whether the current body velocity is considered 'moving'.
    /// </summary>
    /// <returns>true if velocity is significant</returns>
    private bool IsMoving()
    {
        return Vector2.Distance(body.velocity, zeroVector) > 0.2f;
    }
}
