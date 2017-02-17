using UnityEngine;


public class Enemy : BaseEntity
{
    protected Behavior behavior;
    protected Seeker seeker;

    protected AnimatorStateInfo currentStateInfo;
    protected bool passive = true;

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

        // Always face player if not passive, attacking or fleeing
        if (!animator.GetBool("Passive") && !animator.GetBool("Attacking"))
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
        StartCoroutine(LerpSpriteColor(new Color(0.6f, 0.6f, 0.6f), 2));
    }

    public void DisableDeath()
    {
        gameObject.layer = 0;
        StartCoroutine(LerpSpriteColor(Color.white, 2));
        animator.SetBool("Dead", false);
    }

    public void SetEnablePhysics()
    {
        // Enable physics by assigning enemy back to EnemyLayer
        gameObject.layer = 16;
    }


    public Rigidbody2D GetTarget()
    {
        return target;
    }
}
