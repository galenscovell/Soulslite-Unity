using System.Collections;
using UnityEngine;


public class BaseEntity : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D body;
    protected SpriteRenderer spriteRenderer;

    protected bool canMove = true;
    protected bool mirrored = false;
    protected float speedMultiplier;

    protected AudioSource[] soundEffects;

    public float normalSpeed;

    [HideInInspector]
    public Vector2 facingDirection = Vector2.zero;
    [HideInInspector]
    public Vector2 nextVelocity = Vector2.zero;
    

    /**************************
     *          Init          *
     **************************/
    protected void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        soundEffects = GetComponents<AudioSource>();
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
            SetNextVelocity(Vector2.zero);
        }

        // Set new body velocity based on updated nextVelocity
        body.velocity = nextVelocity.normalized * speedMultiplier;
    }


    /**************************
     *          Util          *
     **************************/
    protected void SetFacingDirection()
    {
        facingDirection = nextVelocity.normalized;

        if (mirrored) animator.SetFloat("DirectionX", -facingDirection.x);
        else animator.SetFloat("DirectionX", facingDirection.x);

        animator.SetFloat("DirectionY", facingDirection.y);
    }

    protected bool IsMoving()
    {
        return Vector2.Distance(nextVelocity, Vector2.zero) > 0.2f;
    }


    /**************************
     *          Hurt          *
     **************************/
    protected void Hurt()
    {
        StartCoroutine(HurtFlash());
    }

    protected IEnumerator HurtFlash()
    {
        spriteRenderer.material.SetFloat("_FlashAmount", 0.9f);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.SetFloat("_FlashAmount", 0f);
    }


    /**************************
     *        Getters         *
     **************************/
    public Rigidbody2D GetBody()
    {
        return body;
    }

    public float GetSpeed()
    {
        return speedMultiplier;
    }

    public float GetNormalSpeed()
    {
        return normalSpeed;
    }

    public bool AbleToMove()
    {
        return canMove;
    }


    /**************************
     *        Setters         *
     **************************/
    public void DisableMotion()
    {
        canMove = false;
    }

    public void EnableMotion()
    {
        canMove = true;
    }

    public void SetSpeed(float speed)
    {
        speedMultiplier = speed;
    }

    public void SetNextVelocity(Vector2 velocity)
    {
        nextVelocity = velocity;
    }

    public void EnableMirrored()
    {
        mirrored = true;
    }

    public void DisableMirrored()
    {
        mirrored = false;
    }
}
