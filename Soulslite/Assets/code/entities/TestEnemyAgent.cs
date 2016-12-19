using UnityEngine;


public class TestEnemyAgent : BaseEntity
{
    private SeekBehavior behavior;
    private Seeker seeker;

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
        behavior.WalkPath(body, nextVelocity, speedMultiplier);
        Debug.Log(nextVelocity);
        Debug.Log(body.velocity);

        base.FixedUpdate();
    }



    /**************************
     *      Collisions        *
     **************************/
    private new void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
}
