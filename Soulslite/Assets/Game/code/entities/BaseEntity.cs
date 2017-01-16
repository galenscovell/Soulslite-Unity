using System.Collections;
using UnityEngine;


public class BaseEntity : MonoBehaviour
{
    protected Animator animator;
    protected AudioSource audioSource;
    protected Rigidbody2D body;
    protected SpriteRenderer spriteRenderer;

    public AudioClip[] soundEffects;
    protected bool canMove = true;
    protected float speed;

    // Health
    public int maxHealth;
    protected int health;
    protected bool dead;

    public bool flipX = false;
    public float defaultSpeed;

    [HideInInspector]
    public Vector2 facingDirection;
    [HideInInspector]
    public Vector2 nextVelocity;
    

    /**************************
     *          Init          *
     **************************/
    protected void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (flipX) EnableFlippedX();

        health = maxHealth;

        facingDirection = new Vector2(
            animator.GetFloat("DirectionX"),
            animator.GetFloat("DirectionY")
        );
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
        if (canMove && !dead)
        {
            // If moving, set direction moved in
            if (IsMoving())
            {
                SetFacingDirection(nextVelocity);
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
        body.velocity = nextVelocity.normalized * speed;
    }


    /**************************
     *          Util          *
     **************************/
    protected void SetFacingDirection(Vector2 direction)
    {
        facingDirection = direction.normalized;

        animator.SetFloat("DirectionX", facingDirection.x);
        animator.SetFloat("DirectionY", facingDirection.y);
    }

    protected bool IsMoving()
    {
        return Vector2.Distance(nextVelocity, Vector2.zero) > 0.2f;
    }

    protected void SnapFacingDirection8Way()
    {
        float facingAngle = 0f;
        if (facingDirection.x < 0)
        {
            facingAngle = 360 - (Mathf.Atan2(facingDirection.x, facingDirection.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            facingAngle = Mathf.Atan2(facingDirection.x, facingDirection.y) * Mathf.Rad2Deg;
        }

        if (facingAngle > 315 && facingAngle < 45)
        {
            SetFacingDirection(new Vector2(0, 1));
        }
        else if (facingAngle > 0 && facingAngle < 90)
        {
            SetFacingDirection(new Vector2(1, 1));
        }
        else if (facingAngle > 45 && facingAngle < 135)
        {
            SetFacingDirection(new Vector2(1, 0));
        }
        else if (facingAngle > 90 && facingAngle < 180)
        {
            SetFacingDirection(new Vector2(1, -1));
        }
        else if (facingAngle > 135 && facingAngle < 225)
        {
            SetFacingDirection(new Vector2(0, -1));
        }
        else if (facingAngle > 180 && facingAngle < 270)
        {
            SetFacingDirection(new Vector2(-1, -1));
        }
        else if (facingAngle > 225 && facingAngle < 315)
        {
            SetFacingDirection(new Vector2(-1, 0));
        }
        else if (facingAngle > 270 && facingAngle < 360)
        {
            SetFacingDirection(new Vector2(-1, 1));
        }
    }


    /**************************
     *         Health         *
     **************************/
    protected void DecreaseHealth(int amount)
    {
        health -= amount;
        if (health < 0)
        {
            health = 0;
        }
    }

    protected void IncreaseHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    protected bool HealthZero()
    {
        return health <= 0;
    }


    /**************************
     *         Death          *
     **************************/
    protected void EnableDeath()
    {
        dead = true;
        gameObject.layer = 10;
        spriteRenderer.material.color = new Color(0.65f, 0.65f, 0.65f);
        animator.SetBool("Dead", true);
    }

    protected void DisableDeath()
    {
        DisableFlippedX();
        dead = false;
        gameObject.layer = 0;
        animator.SetBool("Dead", false);
    }

    protected bool IsDead()
    {
        return dead == true;
    }


    /**************************
     *          Hurt          *
     **************************/
    protected void Hurt(Vector2 collisionDirection)
    {
        BloodSystem.bloodSystem.SpawnBlood(body.position, collisionDirection);
        StartCoroutine(HurtFlash());
    }

    protected IEnumerator HurtFlash()
    {
        spriteRenderer.material.SetFloat("_FlashAmount", 0.9f);
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.material.SetFloat("_FlashAmount", 0f);
    }


    /**************************
     *         Audio          *
     **************************/
    public void PlaySfx(int index, float pitch, float volume)
    {
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.PlayOneShot(soundEffects[index]);
    }

    public void PlaySfxRandomPitch(int index, float low, float high, float volume)
    {
        audioSource.pitch = Random.Range(low, high);
        audioSource.volume = volume;
        audioSource.PlayOneShot(soundEffects[index]);
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
        return speed;
    }

    public float GetDefaultSpeed()
    {
        return defaultSpeed;
    }

    public bool AbleToMove()
    {
        return canMove;
    }

    public bool HasFlippedX()
    {
        return spriteRenderer.flipX == true;
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

    public void SetSpeed(float s)
    {
        speed = s;
    }

    public void SetNextVelocity(Vector2 velocity)
    {
        nextVelocity = velocity;
    }

    public void EnableFlippedX()
    {
        spriteRenderer.flipX = true;
    }

    public void DisableFlippedX()
    {
        spriteRenderer.flipX = false;
    }
}
