using UnityEngine;


public class TestEnemyAgent : BaseEntity
{
    private SeekBehavior behavior;
    private Seeker seeker;

    private bool attacking = false;

    public Transform target;



    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();

        behavior = new SeekBehavior();
        seeker = GetComponent<Seeker>();

        behavior.SetPath(seeker, body.position, target.position);
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
        speedMultiplier = 40f;
        nextVelocity = behavior.WalkPath(body, speedMultiplier);
        body.velocity = nextVelocity;

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
        nextVelocity.x = 0;
        nextVelocity.y = 0;
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
        nextVelocity.x = 0;
        nextVelocity.y = 0;
        body.velocity = nextVelocity;
        attacking = false;
    }
}
