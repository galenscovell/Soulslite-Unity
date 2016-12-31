using UnityEngine;


public class Enemy : BaseEntity
{
    protected AnimatorStateInfo currentStateInfo;
    protected bool passive = true;

    protected int attackCounter;
    protected int idleCounter;
    protected int repathCounter;

    public Rigidbody2D target;
    public int repathRate;
    public float pathTracking;
    public int visionDistance;
    public int attackDistance;

    [HideInInspector]
    public Vector2 directionToTarget;


    /**************************
     *          Init          *
     **************************/
    protected new void Start()
    {
        base.Start();

        attackCounter = Random.Range(30, 120);
        idleCounter = Random.Range(90, 360);
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


    /**************************
     *         Ranges         *
     **************************/
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected bool TargetInView()
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
     *        Attack          *
     **************************/
    protected bool AttackCheck()
    {
        attackCounter--;
        if (attackCounter <= 0)
        {
            attackCounter = Random.Range(30, 120);
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

    protected void EndIdleAnim()
    {
        animator.SetBool("IdleAnim", false);
    }
}
