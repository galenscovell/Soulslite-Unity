using UnityEngine;


public class TestEnemyAgent : BaseEntity
{
    private SeekBehavior behavior;
    private Seeker seeker;

    private bool attacking = false;
    private int thoughtTick = 20;
    private Vector2 directionToTarget;

    public Rigidbody2D target;
    public float thinkTime;
    public int aggroDistance;
    public int deAggroDistance;
    public int attackDistance;



    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();

        behavior = new SeekBehavior();
        seeker = GetComponent<Seeker>();
    }



    /**************************
     *        Update          *
     **************************/
    private new void Update()
    {
        base.Update();

        thoughtTick--;
    }

    private new void FixedUpdate()
    {
        /*****************
         * DEFAULT
         *****************/
        if (!attacking)
        {
            if (InAggroRange())
            {
                if (thoughtTick <= 0)
                {
                    behavior.SetPath(seeker, body.position, target, thinkTime);
                    thoughtTick = 20;
                }
            }
            else
            {
                behavior.RemovePath();
            }

            if (behavior.HasPath())
            {
                if (!InAttackRange())
                {
                    if (behavior.HasReachedWaypoint(body.position))
                    {
                        speedMultiplier = 40f;
                        behavior.IncrementPath();
                        Vector2 nextWaypoint = behavior.GetNextWaypoint();
                        Vector2 dirToWaypoint = (nextWaypoint - body.position).normalized;
                        nextVelocity = new Vector2(dirToWaypoint.x * speedMultiplier, dirToWaypoint.y * speedMultiplier);
                    }
                }
                else
                {
                    attacking = true;
                }
            }
            else
            {
                behavior.RemovePath();
                nextVelocity = Vector2.zero;
            }
        }

        animator.SetBool("Attacking", attacking);

        base.FixedUpdate();
    }



    /**************************
     *      Collisions        *
     **************************/
    private new void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }



    /**************************
     *        Ranges          *
     **************************/
    private bool InAggroRange()
    {
        return Vector2.Distance(body.position, target.position) < aggroDistance;
    }

    private bool InAttackRange()
    {
        return Vector2.Distance(body.position, target.position) < attackDistance;
    }



    /**************************
    *         Attack          *
    **************************/
    /// <summary>
    /// 
    /// </summary>
    private void StartAttack()
    {
        nextVelocity = Vector2.zero;
        directionToTarget = (target.position - body.position).normalized;
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartLeap()
    {
        speedMultiplier = 220f;
        nextVelocity = directionToTarget * speedMultiplier;
    }

    private void EndLeap()
    {
        nextVelocity = Vector2.zero;
        directionToTarget = Vector2.zero;
    }

    /// <summary>
    /// 
    /// </summary>
    private void EndAttack()
    {
        nextVelocity = Vector2.zero;
        attacking = false;
    }
}
