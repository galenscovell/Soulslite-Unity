using System.Collections;
using UnityEngine;


public class BaseEntity : MonoBehaviour
{
    protected Animator animator;
    protected AudioSource audioSource;
    protected Rigidbody2D body;
    protected SpriteRenderer spriteRenderer;

    protected bool canMove = true;
    protected float speed;
    protected Vector2 nextVelocity;
    protected Vector2 facingDirection;

    public AudioClip[] soundEffects;
    public bool flipX = false;
    public float defaultSpeed;

    // Health
    public int maxHealth;
    protected int health;


    /**************************
     *          Init          *
     **************************/
    protected void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void Start()
    {
        // Determine entity facing direction on spawn
        if (flipX) EnableFlippedX();
        facingDirection = new Vector2(
            animator.GetFloat("DirectionX"),
            animator.GetFloat("DirectionY")
        );

        health = maxHealth;
    }


    /**************************
     *        Update          *
     **************************/
    protected void Update()
    {
        int normalSortingOrder = -Mathf.RoundToInt((transform.position.y + -0.1f) / 0.05f);
        SetSortingOrder(normalSortingOrder);
    }

    protected void FixedUpdate()
    {
        // Entity is allowed to move
        if (canMove && !IsDead())
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

        // Set new body velocity directly based on nextVelocity
        body.velocity = nextVelocity.normalized * speed;
    }


    /**************************
     *          Util          *
     **************************/
    public void SetFacingDirection(Vector2 direction)
    {
        facingDirection = direction.normalized;

        animator.SetFloat("DirectionX", facingDirection.x);
        animator.SetFloat("DirectionY", facingDirection.y);
    }

    protected bool IsMoving()
    {
        return Vector2.Distance(nextVelocity, Vector2.zero) > 0.1f;
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

    protected int GetHealth()
    {
        return health;
    }


    /**************************
     *         Death          *
     **************************/
    protected void BeginDeath()
    {
        gameObject.layer = 10;
    }

    protected void EnableDeath()
    {
        animator.SetBool("Dead", true);
        spriteRenderer.material.color = new Color(0.7f, 0.7f, 0.7f);
    }

    protected void DisableDeath()
    {
        gameObject.layer = 0;
        spriteRenderer.material.color = new Color(1, 1, 1);
        animator.SetBool("Dead", false);
    }

    protected bool IsDead()
    {
        return animator.GetBool("Dead");
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
     *       Gfx Effects      *
     **************************/
    public void FadeOutSprite()
    {
        LeanTween.value(gameObject, SetSpriteAlpha, 1, 0, 1);
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

    public bool AbleToMove()
    {
        return canMove;
    }

    public bool HasFlippedX()
    {
        return spriteRenderer.flipX == true;
    }

    public Vector2 GetNextVelocity()
    {
        return nextVelocity;
    }

    public Vector2 GetFacingDirection()
    {
        return facingDirection;
    }

    public float GetDefaultSpeed()
    {
        return defaultSpeed;
    }


    /**************************
     *        Setters         *
     **************************/
    public void SetSpriteAlpha(float val)
    {
        spriteRenderer.material.color = new Color(1, 1, 1, val);
    }

    public void DisableMotion()
    {
        canMove = false;
        animator.SetBool("Moving", false);
    }

    public void EnableMotion()
    {
        canMove = true;
    }

    public void SetSpeed(float s)
    {
        speed = s;
    }

    public void RestoreDefaultSpeed()
    {
        speed = defaultSpeed;
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

    public void SetSortingLayer(string value)
    {
        spriteRenderer.sortingLayerName = value;
    }

    public void SetSortingOrder(int value)
    {
        spriteRenderer.sortingOrder = value;
    }
}
