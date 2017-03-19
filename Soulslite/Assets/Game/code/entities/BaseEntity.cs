using System.Collections;
using UnityEngine;


public class BaseEntity : MonoBehaviour
{
    protected Animator animator;
    protected AudioSource audioSource;
    protected Rigidbody2D body;
    protected SpriteRenderer spriteRenderer;

    protected bool canMove = true;
    protected bool falling;
    protected float speed;
    protected Vector2 nextVelocity;
    protected Vector2 facingDirection;
    protected Vector2 movementImpulse;
    protected float movementImpulseTime;

    public AudioClip[] soundEffects;
    public BlobShadow blobShadow;
    public bool flipX;
    public float defaultSpeed;

    public int maxHealth;
    protected int currentHealth;


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

        currentHealth = maxHealth;
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
        // Update body position for hurt impulse
        ApplyMovementImpulse();
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
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    protected void IncreaseHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    protected bool HealthZero()
    {
        return currentHealth <= 0;
    }

    protected void RestoreFullHealth()
    {
        currentHealth = maxHealth;
    }

    protected int GetCurrentHealth()
    {
        return currentHealth;
    }


    /**************************
     *         Death          *
     **************************/
    public void IgnoreAllCollisions()
    {
        gameObject.layer = 10;
    }

    public void IgnoreEntityCollisions()
    {
        gameObject.layer = 18;
    }

    protected bool IsDead()
    {
        return animator.GetBool("Dead");
    }


    /**************************
     *          Hurt          *
     **************************/
    protected void Hurt(Vector2 collisionDirection, float recoil)
    {
        SetMovementImpulse(collisionDirection.normalized, recoil, 0.1f);
        BloodSystem.bloodSystem.SpawnBlood(body.position, collisionDirection);
        StartCoroutine(FlashSpriteColor(Color.white, 0.05f, 1f, 0.4f, 0));
        CameraSystem.cameraSystem.ActivateShake(recoil - 1, 0.1f);
    }

    protected void ApplyMovementImpulse()
    {
        if (movementImpulseTime > 0)
        {
            movementImpulseTime -= Time.deltaTime;
            body.MovePosition(body.position + movementImpulse);
        }
    }

    public void SetMovementImpulse(Vector2 direction, float force, float time)
    {
        movementImpulse = direction * force;
        movementImpulseTime = time;
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
    public void FadeOutSprite(float overTime)
    {
        LeanTween.value(gameObject, SetSpriteAlpha, 1, 0, overTime);
    }

    public void FadeInSprite(float overTime)
    {
        LeanTween.value(gameObject, SetSpriteAlpha, 0, 1, overTime);
    }

    public void TweenSpeed(float start, float end, float overTime)
    {
        LeanTween.value(gameObject, SetSpeed, start, end, overTime);
    }

    public IEnumerator ChangeSpriteColorInto(Color blinkColor, float runtime, float targetAlpha)
    {
        float time = 0;
        float startAlpha = 0;

        while (time < 1)
        {
            time += Time.deltaTime / runtime;
            blinkColor.a = Mathf.Lerp(startAlpha, targetAlpha, time);
            spriteRenderer.material.SetColor("_BlinkColor", blinkColor);
            yield return null;
        }
    }

    public IEnumerator ChangeSpriteColorOutof(Color blinkColor, float runtime, float targetAlpha)
    {
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime / runtime;
            spriteRenderer.material.SetColor("_BlinkColor", blinkColor);
            blinkColor.a = Mathf.Lerp(blinkColor.a, targetAlpha, time);
            yield return null;
        }
    }

    public IEnumerator FlashSpriteColor(Color color, float inTime, float inAlpha, float outTime, float outAlpha)
    {
        yield return ChangeSpriteColorInto(color, inTime, inAlpha);
        yield return ChangeSpriteColorOutof(color, outTime, outAlpha);
    }


    /**************************
     *         Shadow         *
     **************************/
    public BlobShadow GetShadow()
    {
        return blobShadow;
    }


    /**************************
     *        Getters         *
     **************************/
    public Rigidbody2D GetBody()
    {
        return body;
    }

    public Vector2 GetCenter()
    {
        return spriteRenderer.bounds.center;
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

    public bool IsFalling()
    {
        return falling;
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

    public void SetFalling(bool setting)
    {
        falling = setting;
    }
}
