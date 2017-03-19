using UnityEngine;


public class MinibossAgent : Enemy
{
    // State machines
    private MinibossAttack attack;
    private MinibossDying dying;
    private MinibossEntrance entrance;
    private MinibossJump jump;
    private MinibossMovement movement;
    private MinibossRoar roar;

    private int idleStateHash = Animator.StringToHash("Base Layer.MinibossIdle");

    public int jumpRate;
    public BlobShadow jumpShadow;

    private int jumpCounter = 0;
    private int randomizedJumpRate;
    private int jumpCount = 0;
    private int roarThreshold = 3;


    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();

        behavior = new Behavior();
        seeker = GetComponent<Seeker>();

        attack = animator.GetBehaviour<MinibossAttack>();
        dying = animator.GetBehaviour<MinibossDying>();
        entrance = animator.GetBehaviour<MinibossEntrance>();
        jump = animator.GetBehaviour<MinibossJump>();
        movement = animator.GetBehaviour<MinibossMovement>();
        roar = animator.GetBehaviour<MinibossRoar>();

        fall = animator.GetBehaviour<EnemyFall>();
        fullIdle = animator.GetBehaviour<EnemyFullIdle>();

        // Ints in state Setups are the sfx index
        attack.Setup(this, 0);
        dying.Setup(this, 1);
        jump.Setup(this, new int[2] { 2, 3 }, jumpShadow);
        entrance.Setup(this, new int[2] { 3, 4 }, jumpShadow);
        movement.Setup(this, 5);
        roar.Setup(this, 4);
        
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
        bool roarReady = RoarCheck();
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
                if (jump.CheckTargetRange(TrackTarget()))
                {
                    jumpCount++;
                    FaceTarget();
                    animator.SetBool("Jumping", true);
                    return;
                }
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
            jumpCounter = (int) Random.Range(jumpRate * 0.5f, jumpRate);
            return true;
        }
        return false;
    }

    protected bool RoarCheck()
    {
        if (jumpCount >= roarThreshold)
        {
            animator.SetBool("Roaring", true);
            jumpCount = 0;
            roarThreshold = Random.Range(3, 6);
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
                    TakeBulletHit(1, 0, collisionDirection);
                    UISystem.uiSystem.UpdateBossHealth(-1);
                    break;
                case "PlayerAttack":
                    TakeNormalHit(1, 0, collisionDirection);
                    UISystem.uiSystem.UpdateBossHealth(-1);
                    break;
                case "PlayerStrongAttack":
                    TakeStrongHit(2, 0, collisionDirection);
                    UISystem.uiSystem.UpdateBossHealth(-2);
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
