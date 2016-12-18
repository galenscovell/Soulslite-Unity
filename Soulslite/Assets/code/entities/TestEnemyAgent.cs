using UnityEngine;

public class TestEnemyAgent : BaseEntity
{



    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();
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
