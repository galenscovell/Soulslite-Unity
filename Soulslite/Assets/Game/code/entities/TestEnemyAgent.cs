using UnityEngine;


public class TestEnemyAgent : Enemy
{
    private SeekBehavior behavior;
    private Seeker seeker;

    // State machines
    private TestenemyAttack attack;
    private TestenemyHurt hurt;


    /**************************
     *          Init          *
     **************************/
    private new void Start() 
    {
        base.Start();

        behavior = new SeekBehavior();
        seeker = GetComponent<Seeker>();

        attack = animator.GetBehaviour<TestenemyAttack>();
        hurt = animator.GetBehaviour<TestenemyHurt>();

        // Ints in state Setups are the sfx index
        attack.Setup(this, 0);
        hurt.Setup(this);
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
        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!IsDead())
        {
            /****************
             * PASSIVE
             ****************/
            if (passive)
            {
                if (IdleAnimCheck())
                {
                    animator.Play("TestenemyIdleAnim");
                }

                if (TargetInView())
                {
                    if (HasFlippedX()) DisableFlippedX();

                    passive = false;
                    EndIdleAnim();
                }
            }
            /****************
             * ACTIVE
             ****************/
            else
            {
                bool attackReady = AttackCheck();

                /****************
                 * NOT ATTACKING
                 ****************/
                if (currentStateInfo.fullPathHash != attack.GetHash())
                {
                    /****************
                     * REPATH
                     ****************/
                    if (repathCounter <= 0)
                    {
                        repathCounter = repathRate;
                        behavior.SetPath(seeker, body.position, TrackTarget());
                    }

                    /****************
                     * FOLLOWING
                     ****************/
                    if (behavior.HasPath())
                    {
                        /****************
                         * START ATTACK
                         ****************/
                        if (InAttackRange() && TargetInView() && attackReady)
                        {
                            animator.SetBool("Attacking", true);
                        }
                        /****************
                         * CONTINUE
                         ****************/
                        else
                        {
                            if (behavior.WaypointReached(body.position))
                            {
                                speed = defaultSpeed;
                                Vector2 nextWaypoint = behavior.GetNextWaypoint();

                                if (nextWaypoint != null)
                                {
                                    Vector2 dirToWaypoint = (behavior.GetNextWaypoint() - body.position).normalized;
                                    SetNextVelocity(dirToWaypoint * speed);
                                }
                            }
                        }
                    }
                    /****************
                     * NO PATH
                     ****************/
                    else
                    {
                        SetNextVelocity(Vector2.zero);
                    }
                }
            }

            // Will update body velocity and facing direction
            base.FixedUpdate();
        }
    }


    /**************************
     *       Collision        *
     **************************/
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player-attack")
        {
            if (currentStateInfo.fullPathHash == hurt.GetHash())
            {
                return;
            }

            Vector2 collisionDirection = transform.position - collision.transform.position;

            Hurt(collisionDirection);
            DecreaseHealth(1);

            if (HealthZero() && !IsDead())
            {
                hurt.SetFlungVelocity(collision.attachedRigidbody.velocity.normalized);
                animator.Play(hurt.GetHash());
            }
        }
    }


    /**************************
     *         Hurt          *
     **************************/
    private new void Hurt(Vector2 collisionDirection)
    {
        // Flash, particle fx and damage
        base.Hurt(collisionDirection);
        CameraController.cameraController.ActivateShake();
    }


    /**************************
     *         Death          *
     **************************/
    public void Die()
    {
        EnableDeath();
        animator.SetBool("Attacking", false);
        animator.SetBool("Dead", true);
    }
}
