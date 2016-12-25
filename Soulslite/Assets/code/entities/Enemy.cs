using UnityEngine;


public class Enemy : BaseEntity
{
    protected bool passive = true;
    protected bool attacking = false;
    protected Vector2 directionToTarget;

    protected int idleCounter = Random.Range(90, 360);
    protected int repathCounter;

    public Rigidbody2D target;
    public int repathRate;
    public float pathTracking;
    public int visionDistance;
    public int attackDistance;


    /**************************
     *          Init          *
     **************************/
    protected new void Start()
    {
        base.Start();
        repathCounter = repathRate;
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
        repathCounter--;
    }


    /**************************
     *         Senses         *
     **************************/
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected Vector2 TrackTarget()
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


    /**************************
     *         Ranges         *
     **************************/
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected bool TargetInVision()
    {
        Vector2 rayDirection = target.position - body.position;
        RaycastHit2D hit = Physics2D.Raycast(body.position, rayDirection, visionDistance);
        return hit && hit.collider.tag == target.tag;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected bool InAttackRange()
    {
        return Vector2.Distance(body.position, target.position) < attackDistance;
    }


    /**************************
     *          Idle          *
     **************************/
    protected void IdleAnimCheck()
    {
        idleCounter--;
        if (idleCounter <= 0)
        {
            animator.SetBool("IdleAnim", true);
            idleCounter = Random.Range(120, 360);
        }
    }

    protected void EndIdleAnim()
    {
        animator.SetBool("IdleAnim", false);
    }
}
