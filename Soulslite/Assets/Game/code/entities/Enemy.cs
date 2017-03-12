using UnityEngine;


public class Enemy : BaseEntity
{
    protected Behavior behavior;
    protected Seeker seeker;

    protected AnimatorStateInfo currentStateInfo;
    protected bool passive = true;
    protected EnemyFall fall;
    protected EnemyFullIdle fullIdle;

    protected int attackCounter = 0;
    protected int idleCounter = 0;
    protected int repathCounter;
    protected int wanderCounter = 0;

    public int repathRate;
    public int attackRate;
    public int wanderRate;
    public float pathTracking;
    public int visionRange;
    public int attackRange;

    [HideInInspector]
    public Vector2 directionToTarget;

    private Rigidbody2D target;
    private LayerMask visionLayer;
    private LayerMask enemyLayer;


    /**************************
     *          Init          *
     **************************/
    protected new void Start()
    {
        base.Start();

        repathCounter = repathRate;
        target = LevelSystem.levelSystem.player.GetBody();
        visionLayer = (1 << LayerMask.NameToLayer("ObstacleLayer") | 1 << LayerMask.NameToLayer("PlayerLayer"));
        enemyLayer = 1 << LayerMask.NameToLayer("EnemyLayer");
    }


    /**************************
     *        Update          *
     **************************/
    protected new void Update()
    {
        base.Update();
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();

        // Always face player if not dead, passive, attacking or fleeing
        if (!animator.GetBool("Passive") && !animator.GetBool("Attacking") && !IsDead())
        {
            FaceTarget();
        }

        // Enemies all operate on x axis only, so we can mirror at all times
        // If x is negative, completely flip entity to mirror colliders and animations
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
     *         Senses         *
     **************************/
    public Vector2 TrackTarget()
    {
        Vector2 trackedPosition = Vector2.zero;
        if (pathTracking > 0)
        {
            Vector2 currentTargetVelocity = target.velocity;
            trackedPosition = target.position + (target.velocity * pathTracking);
        }
        else
        {
            trackedPosition = target.position;
        }

        return trackedPosition;
    }

    public void FaceTarget()
    {
        Vector2 dirToTarget = (target.position - body.position).normalized;
        SetFacingDirection(dirToTarget);
    }

    protected bool TargetInView()
    {
        Vector2 rayDirection = target.position - body.position;
        RaycastHit2D hit = Physics2D.Raycast(body.position, rayDirection, visionRange, visionLayer.value);
        return hit && hit.collider.tag == target.tag;
    }

    protected bool VisionBlockedByEnemy()
    {
        Vector2 rayDirection = target.position - body.position;
        RaycastHit2D hit = Physics2D.Raycast(body.position, rayDirection, visionRange, enemyLayer.value);
        return hit;
    }

    protected bool InAttackRange()
    {
        float distance = Vector2.Distance(body.position, target.position);
        return distance < attackRange;
    }


    /**************************
     *        Attack          *
     **************************/
    protected bool AttackCheck()
    {
        attackCounter--;
        if (attackCounter <= 0)
        {
            attackCounter = Random.Range(attackRate - 5, attackRate + 5);
            return true;
        }
        return false;
    }


    /**************************
     *          Idle          *
     **************************/
    protected bool IdleAnimCheck()
    {
        idleCounter--;
        if (idleCounter <= 0)
        {
            idleCounter = Random.Range(120, 360);
            return true;
        }
        return false;
    }


    /**************************
     *         Wander         *
     **************************/
    protected bool WanderCheck()
    {
        wanderCounter--;
        if (wanderCounter <= 0)
        {
            wanderCounter = Random.Range(wanderRate - 5, wanderRate + 5);
            return true;
        }
        return false;
    }


    /**************************
     *         Death          *
     **************************/
    public void EnableDeath()
    {
        animator.SetBool("Attacking", false);
        animator.SetBool("Dead", true);
        StartCoroutine(ChangeSpriteColorInto(Color.black, 1f, 0.4f));
        IgnoreAllCollisions();
    }

    public void DisableDeath()
    {
        RestoreCollisions();
        SetFalling(false);
        StartCoroutine(ChangeSpriteColorOutof(Color.black, 1f, 0));
        animator.SetBool("Dead", false);
    }

    public void RestoreCollisions()
    {
        // Enable physics by assigning enemy back to EnemyLayer
        gameObject.layer = 16;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }


    /**************************
     *        Getters         *
     **************************/
    public Rigidbody2D GetTarget()
    {
        return target;
    }


    /**************************
     *       Collision        *
     **************************/
    protected void OnCollisionEnter2D(Collision2D collision)
    {

    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore collisions with critters
        if (collision.tag == "CritterTag") return;
        
        // Set as ready to fall if colliding with falloff boundary
        if (collision.tag == "FalloffTag")
        {
            SetFalling(true);
            animator.Play(fall.GetHash());
            return;
        }
    }

    protected void TakeNormalHit(int damage, Vector2 collisionDirection)
    {
        UISystem.uiSystem.UpdateAmmo(1);
        Hurt(collisionDirection, 3);
        DecreaseHealth(damage);
    }

    protected void TakeStrongHit(int damage, Vector2 collisionDirection)
    {
        UISystem.uiSystem.UpdateAmmo(1);
        Hurt(collisionDirection, 5);
        DecreaseHealth(damage);
    }

    protected void TakeBulletHit(int damage, Vector2 collisionDirection)
    {
        Hurt(collisionDirection, 2);
        DecreaseHealth(damage);
    }
}
