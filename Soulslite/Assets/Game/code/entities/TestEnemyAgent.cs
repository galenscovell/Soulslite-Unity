using UnityEngine;


public class TestEnemyAgent : Enemy
{
    private SeekBehavior behavior;
    private Seeker seeker;

    private TestenemyAttackState attackState;
    private int attackStateHash = Animator.StringToHash("Base Layer.TestenemyAttackState");

    private TestenemyHurtState hurtState;
    private int hurtStateHash = Animator.StringToHash("Base Layer.TestenemyHurtState");

    private CameraShaker cameraShaker;


    /**************************
     *          Init          *
     **************************/
    private new void Start() 
    {
        base.Start();

        behavior = new SeekBehavior();
        seeker = GetComponent<Seeker>();

        AudioSource attackSound = soundEffects[0];

        attackState = animator.GetBehaviour<TestenemyAttackState>();
        attackState.Setup(this, attackSound);

        hurtState = animator.GetBehaviour<TestenemyHurtState>();
        hurtState.Setup(this);

        cameraShaker = GetComponent<CameraShaker>();
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

        /****************
         * PASSIVE
         ****************/
        if (passive)
        {
            if (IdleAnimCheck())
            {
                animator.SetBool("IdleAnim", true);
            }

            if (TargetInView())
            {
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
            if (currentStateInfo.fullPathHash != attackStateHash)
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
                            speedMultiplier = normalSpeed;
                            Vector2 nextWaypoint = behavior.GetNextWaypoint();

                            if (nextWaypoint != null)
                            {
                                Vector2 dirToWaypoint = (behavior.GetNextWaypoint() - body.position).normalized;
                                SetNextVelocity(dirToWaypoint * speedMultiplier);
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
            if (currentStateInfo.fullPathHash == hurtStateHash)
            {
                return;
            }

            hurtState.SetHurtVelocity(collision.attachedRigidbody.velocity.normalized);
            Hurt();
        }
    }


    /**************************
     *         Hurt          *
     **************************/
    private new void Hurt()
    {
        // Flash and damage
        base.Hurt();
        cameraShaker.Activate();

        // If in non-interruptible state, do not change to hurt state
        if (currentStateInfo.fullPathHash == attackStateHash && !attackState.Interrupt(animator))
        {
            return;
        }

        // Change to hurt state
        animator.Play("TestenemyHurtState");
    }
}
