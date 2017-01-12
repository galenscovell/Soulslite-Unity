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

        attackCounter = Random.Range(60, 90);
        idleCounter = Random.Range(120, 360);
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

        repathCounter--;
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


    /**************************
     *         Ranges         *
     **************************/
    protected bool TargetInView()
    {
        Vector2 rayDirection = target.position - body.position;
        RaycastHit2D hit = Physics2D.Raycast(body.position, rayDirection, visionDistance);
        return hit && hit.collider.tag == target.tag;
    }

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
            attackCounter = Random.Range(60, 90);
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
