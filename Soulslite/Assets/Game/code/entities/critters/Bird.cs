using System.Collections;
using UnityEngine;


public class Bird : MonoBehaviour
{
    private Animator animator;
    protected SpriteRenderer spriteRenderer;

    private Vector3 facingDirection;

    private bool moving = false;
    private bool centered = true;
    private bool flying = false;

    private float hopHeight = 4f;
    private float speed = 8;

    private float moveRate = 2;
    private float moveCounter;

    private float despawnTime = 8;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        MovementSetter();
        facingDirection = new Vector3(
            animator.GetFloat("DirectionX"),
            animator.GetFloat("DirectionY"),
            0
        );
    }

    /**************************
     *        Update          *
     **************************/
    private void Update()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt((transform.position.y + -0.1f) / 0.05f);
    }

    private void FixedUpdate()
    {
        if (flying)
        {
            transform.Translate(new Vector3(facingDirection.x * speed, facingDirection.y * speed, 0));
            despawnTime -= Time.deltaTime;
            if (despawnTime < 0)
            {
                Destroy(gameObject);
            }
        }
        else if (!moving)
        {
            moveCounter += Time.deltaTime;
            if (moveCounter > moveRate)
            {
                if (centered)
                {
                    facingDirection = GetRandomDirection();
                    centered = false;
                }
                else
                {
                    facingDirection = -facingDirection;
                    centered = true;
                }

                StartCoroutine(Hop(transform.position + (facingDirection * speed), 0.35f));
                moveCounter = 0;
                MovementSetter();
            }
        }


        if (facingDirection.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }


    /**************************
     *          Util          *
     **************************/
    private IEnumerator Hop(Vector3 dest, float time)
    {
        if (moving) yield break;

        moving = true;
        var startPos = transform.position;
        var timer = 0.0f;

        while (timer <= 1.0f)
        {
            var height = Mathf.Sin(Mathf.PI * timer) * hopHeight;
            transform.position = Vector3.Lerp(startPos, dest, timer) + Vector3.up * height;

            timer += Time.deltaTime / time;
            yield return null;
        }
        moving = false;
    }

    private void SetFacingDirection(Vector2 direction)
    {
        facingDirection = direction.normalized;

        animator.SetFloat("DirectionX", facingDirection.x);
        animator.SetFloat("DirectionY", facingDirection.y);
    }

    private void MovementSetter()
    {
        moveRate = Random.Range(0.5f, 3.5f);
    }

    private Vector2 GetRandomDirection()
    {
        return Random.insideUnitCircle.normalized;
    }

    private Vector2 GetFlyingDirection()
    {
        return new Vector2(Random.Range(0, 2) * 2 - 1, 0.75f);
    }

    private void SetSortingLayer(string value)
    {
        spriteRenderer.sortingLayerName = value;
    }

    public void IgnoreAllPhysics()
    {
        gameObject.layer = 10;
    }


    /**************************
     *       Collisions       *
     **************************/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // All collisions spook the bird to fly away
        animator.SetBool("Flying", true);
        facingDirection = GetFlyingDirection();
        speed = 2.5f;
        flying = true;
        SetSortingLayer("Foreground");
        IgnoreAllPhysics();
    }
}
