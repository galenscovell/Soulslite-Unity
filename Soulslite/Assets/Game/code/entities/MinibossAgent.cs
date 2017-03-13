using UnityEngine;


public class MinibossAgent : Enemy
{
    // State machines
    private MinibossAttack attack;
    private MinibossEntrance entrance;
    private MinibossJump jump;
    private MinibossMovement movement;
    private EnemyMeleeDying dying;

    private int idleStateHash = Animator.StringToHash("Base Layer.MinibossIdle");

    private int jumpCounter = 0;
    private int jumpRate = 30;


    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();

        behavior = new Behavior();
        seeker = GetComponent<Seeker>();

        attack = animator.GetBehaviour<MinibossAttack>();
        dying = animator.GetBehaviour<EnemyMeleeDying>();
        entrance = animator.GetBehaviour<MinibossEntrance>();
        jump = animator.GetBehaviour<MinibossJump>();
        movement = animator.GetBehaviour<MinibossMovement>();

        fall = animator.GetBehaviour<EnemyFall>();
        fullIdle = animator.GetBehaviour<EnemyFullIdle>();

        // Ints in state Setups are the sfx index
        attack.Setup(this, 0);
        dying.Setup(this, 1);
        jump.Setup(this, new int[2] { 2, 3 });
        entrance.Setup(this, new int[2] { 3, 4 });
        movement.Setup(this, 5);
        
        fall.Setup(this, 1);
        fullIdle.Setup(this);
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
            ActiveUpdate();

            // Will update body velocity and facing direction
            base.FixedUpdate();
        }
    }


    /**************************
     *      Enemy States      *
     **************************/
    private void ActiveUpdate()
    {
        bool attackReady = AttackCheck();
        bool jumpReady = JumpCheck();
        bool wanderReady = WanderCheck();
        bool inAttackRange = InAttackRange();
        bool targetInView = TargetInView();
        bool visionBlockedByEnemy = VisionBlockedByEnemy();

        if (currentStateInfo.fullPathHash == movement.GetHash() || currentStateInfo.fullPathHash == idleStateHash)
        {
            /****************
             * JUMP ATTACK
             ****************/
            if (jumpReady)
            {
                jump.SetJumpTarget(GetTarget().position);
                animator.SetBool("Jumping", true);
                return;
            }

            /****************
             * REPATH
             ****************/
            repathCounter--;
            if (repathCounter <= 0)
            {
                repathCounter = repathRate;

                if (!targetInView || !inAttackRange)
                {
                    behavior.SetPath(seeker, body.position, TrackTarget());
                }
                else
                {
                    if (!behavior.IsWandering() && behavior.HasPath())
                    {
                        behavior.EndPath();
                    }

                    if (wanderReady)
                    {
                        behavior.Wander(seeker, body.position);
                    }
                }
            }

            /****************
             * ATTACK
             ****************/
            if (inAttackRange && targetInView && attackReady)
            {
                FaceTarget();
                animator.SetBool("Attacking", true);
            }
            else
            {
                /****************
                 * FOLLOWING
                 ****************/
                if (behavior.HasPath())
                {
                    if (behavior.WaypointReached(body.position))
                    {
                        if (behavior.IsWandering())
                        {
                            SetSpeed(defaultSpeed / 2);
                        }
                        else
                        {
                            SetSpeed(defaultSpeed);
                        }

                        behavior.IncrementWaypoint();
                        if (behavior.HasPath())
                        {
                            Vector2 nextWaypoint = behavior.GetNextWaypoint();
                            Vector2 dirToWaypoint = (nextWaypoint - body.position).normalized;
                            SetNextVelocity(dirToWaypoint * speed);
                        }
                        else
                        {
                            SetNextVelocity(Vector2.zero);
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
    }


    protected bool JumpCheck()
    {
        jumpCounter--;
        if (jumpCounter <= 0)
        {
            jumpCounter = Random.Range(jumpRate - 20, jumpRate + 20);
            return true;
        }
        return false;
    }


    /**************************
     *       Collision        *
     **************************/
    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // Enemy is dying but not yet disabled
        if (currentStateInfo.fullPathHash == dying.GetHash())
        {
            // Set as ready to fall if falling off level
            if (collision.tag == "FalloffTag")
            {
                SetFalling(true);
                animator.Play(fall.GetHash());
                return;
            }
        }

        if (collision.gameObject.tag == "PlayerAttack" || collision.gameObject.tag == "PlayerStrongAttack" || collision.gameObject.tag == "PlayerBullet")
        {
            Vector2 collisionDirection = (transform.position - collision.transform.position).normalized;

            switch (collision.gameObject.tag)
            {
                case "PlayerBullet":
                    TakeBulletHit(1, collisionDirection);
                    break;
                case "PlayerAttack":
                    TakeNormalHit(1, collisionDirection);
                    break;
                case "PlayerStrongAttack":
                    TakeStrongHit(2, collisionDirection);
                    break;
                default:
                    break;
            }

            // Check health status
            if (HealthZero() && !IsDead())
            {
                dying.SetFlungVelocity(collisionDirection);
                TimeSystem.timeSystem.SlowTime(0.5f, 1f);
                animator.Play(dying.GetHash());
            }
        }
    }
}
