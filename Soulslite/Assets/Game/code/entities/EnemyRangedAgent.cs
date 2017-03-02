﻿using UnityEngine;


public class EnemyRangedAgent : Enemy
{
    // State machines
    private EnemyRangedAttack attack;
    private EnemyRangedDying dying;


    /**************************
     *          Init          *
     **************************/
    private new void Start()
    {
        base.Start();

        behavior = new Behavior();
        seeker = GetComponent<Seeker>();

        attack = animator.GetBehaviour<EnemyRangedAttack>();
        dying = animator.GetBehaviour<EnemyRangedDying>();
        fall = animator.GetBehaviour<EnemyFall>();
        fullIdle = animator.GetBehaviour<EnemyFullIdle>();

        EnemyRangedGunLimb gunLimb = transform.Find("GunLimb").GetComponent<EnemyRangedGunLimb>();

        // Ints in state Setups are the sfx index
        attack.Setup(this, gunLimb, 0);
        dying.Setup(this, 1);
        fall.Setup(this, 2);
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
            if (animator.GetBool("Passive"))
            {
                PassiveUpdate();
            }
            else
            {
                ActiveUpdate();
            }

            // Will update body velocity and facing direction
            base.FixedUpdate();
        }
    }


    /**************************
     *      Enemy States      *
     **************************/
    private void PassiveUpdate()
    {
        if (IdleAnimCheck())
        {
            animator.SetBool("Idling", true);
        }

        if (TargetInView())
        {
            if (HasFlippedX()) DisableFlippedX();
            animator.SetBool("Passive", false);
            animator.SetBool("Idling", false);
        }
    }

    private void ActiveUpdate()
    {
        bool attackReady = AttackCheck();
        bool wanderReady = WanderCheck();
        bool inAttackRange = InAttackRange();
        bool targetInView = TargetInView();

        if (currentStateInfo.fullPathHash != attack.GetHash())
        {
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
                DisableMotion();
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


    /**************************
     *       Collision        *
     **************************/
    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // Enemy is dying but not yet disabled
        if (currentStateInfo.fullPathHash == dying.GetHash()) return;

        if (collision.gameObject.tag == "PlayerAttack" || collision.gameObject.tag == "PlayerStrongAttack" || collision.gameObject.tag == "PlayerBullet")
        {
            // Hit during attacking state
            if (currentStateInfo.fullPathHash == attack.GetHash() && !attack.Interrupt(animator))
            {
                // Invulnerable state
                return;
            }

            switch (collision.gameObject.tag)
            {
                case "PlayerBullet":
                    TakeBulletHit(1, collision);
                    break;
                case "PlayerAttack":
                    TakeNormalHit(1, collision);
                    break;
                case "PlayerStrongAttack":
                    TakeStrongHit(2, collision);
                    break;
                default:
                    break;
            }

            // Make enemy active if player hits them, even if outside range
            if (passive)
            {
                if (HasFlippedX()) DisableFlippedX();
                animator.SetBool("Passive", false);
                animator.SetBool("Idling", false);
            }

            // Check health status
            if (HealthZero() && !IsDead())
            {
                dying.SetFlungVelocity(collision.attachedRigidbody.velocity.normalized);
                animator.Play(dying.GetHash());
            }
        }
    }
}
