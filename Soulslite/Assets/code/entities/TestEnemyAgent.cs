using UnityEngine;


public class TestEnemyAgent : Enemy
{
    private SeekBehavior behavior;
    private Seeker seeker;


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
    }

    private new void FixedUpdate()
    {
        /*****************
         * PASSIVE
         *****************/
        if (passive)
        {
            IdleAnimCheck();
            if (TargetInVision())
            {
                passive = false;
                EndIdleAnim();
            }
        }
        /*****************
         * ACTIVE
         *****************/
        else
        {
            if (!attacking)
            {
                // Repath
                if (repathCounter <= 0)
                {
                    repathCounter = repathRate;
                    behavior.SetPath(seeker, body.position, TrackTarget());
                }

                // Follow set path
                if (behavior.HasPath())
                {
                    // Target not in attack range or not within vision
                    if (!InAttackRange() || !TargetInVision())
                    {
                        if (behavior.HasReachedWaypoint(body.position))
                        {
                            speedMultiplier = 40f;
                            behavior.IncrementPath();
                            Vector2 dirToWaypoint = (behavior.GetNextWaypoint() - body.position).normalized;
                            nextVelocity = dirToWaypoint * speedMultiplier;
                        }
                    }
                    // Start attack
                    else
                    {
                        attacking = true;
                    }
                }
                // No path to follow, stop motion
                else
                {
                    nextVelocity = Vector2.zero;
                }
            }
        }

        animator.SetBool("Attacking", attacking);

        // Will update body velocity and facing direction
        base.FixedUpdate();
    }


    /**************************
     *       Collision        *
     **************************/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }


   /**************************
    *         Attack         *
    **************************/
    /// <summary>
    /// Enemy prepares for leap (no motion)
    /// </summary>
    private void TestenemyStartAttack()
    {
        nextVelocity = Vector2.zero;
        directionToTarget = (TrackTarget() - body.position).normalized;
    }

    /// <summary>
    /// Enemy begins leap towards tracked target
    /// </summary>
    private void TestenemyStartLeap()
    {
        speedMultiplier = 220f;
        nextVelocity = directionToTarget * speedMultiplier;
    }

    /// <summary>
    /// Enemy lands from leap (no motion)
    /// </summary>
    private void TestenemyEndLeap()
    {
        nextVelocity = Vector2.zero;
        directionToTarget = Vector2.zero;
    }

    /// <summary>
    /// Enemy attack has finished
    /// </summary>
    private void TestenemyEndAttack()
    {
        nextVelocity = Vector2.zero;
        attacking = false;
    }
}
