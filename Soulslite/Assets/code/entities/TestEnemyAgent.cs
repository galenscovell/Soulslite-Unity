using UnityEngine;


public class TestEnemyAgent : BaseEntity
{
    private SeekBehavior behavior;
    private Seeker seeker;

    private bool attacking = false;
    private int thoughtTick = 20;

    public Transform target;



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
        if (behavior.InAggroRange(body.position, target.position))
        {
            if (thoughtTick <= 0)
            {
                behavior.SetPath(seeker, body.position, target.position);
                thoughtTick = 20;
            }
        } else
        {
            behavior.RemovePath();
        }

        if (behavior.HasPath() && !behavior.InAttackRange(body.position))
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
            behavior.RemovePath();
            nextVelocity = Vector2.zero;
        }

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
    *         Attack          *
    **************************/
    /// <summary>
    /// 
    /// </summary>
    private void StartAttack()
    {
        nextVelocity = Vector2.zero;
        body.velocity = nextVelocity;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Attack()
    {
        speedMultiplier = 1200f;
        nextVelocity.x = body.velocity.x * speedMultiplier;
        nextVelocity.y = body.velocity.y * speedMultiplier;
        body.velocity = NormalizedDiagonal(nextVelocity);
    }

    /// <summary>
    /// 
    /// </summary>
    private void EndAttack()
    {
        nextVelocity = Vector2.zero;
        body.velocity = nextVelocity;
        attacking = false;
    }
}
