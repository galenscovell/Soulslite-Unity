﻿using UnityEngine;


public class EnemyMeleeAgent : Enemy
{
    // State machines
    private EnemyMeleeAttack attack;
    private EnemyMeleeHurt hurt;
    private EnemyMeleeFullIdle fullIdle;


    /**************************
     *          Init          *
     **************************/
    private new void Start() 
    {
        base.Start();

        behavior = new Behavior();
        seeker = GetComponent<Seeker>();

        attack = animator.GetBehaviour<EnemyMeleeAttack>();
        hurt = animator.GetBehaviour<EnemyMeleeHurt>();
        fullIdle = animator.GetBehaviour<EnemyMeleeFullIdle>();

        // Ints in state Setups are the sfx index
        attack.Setup(this, 0);
        hurt.Setup(this);
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
                animator.SetBool("Attacking", true);
            }

            /****************
             * FOLLOWING
             ****************/
            if (behavior.HasPath())
            {
                if (behavior.WaypointReached(body.position))
                {
                    if(behavior.IsWandering())
                    {
                        SetSpeed(20);
                    } 
                    else
                    {
                        SetSpeed(defaultSpeed);
                    }

                    Vector2 nextWaypoint = behavior.GetNextWaypoint();

                    if (nextWaypoint != null)
                    {
                        Vector2 dirToWaypoint = (behavior.GetNextWaypoint() - body.position).normalized;
                        SetNextVelocity(dirToWaypoint * speed);
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


    /**************************
     *       Collision        *
     **************************/
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerAttack" || collision.gameObject.tag == "PlayerBullet")
        {
            if (currentStateInfo.fullPathHash == hurt.GetHash())
            {
                return;
            }

            Vector2 collisionDirection = transform.position - collision.transform.position;

            Hurt(collisionDirection);
            DecreaseHealth(1);

            // Make enemy active if player hits them, even if outside range
            if (passive)
            {
                if (HasFlippedX()) DisableFlippedX();
                animator.SetBool("Passive", false);
                animator.SetBool("Idling", false);
            }

            if (HealthZero() && !IsDead())
            {
                SetIgnorePhysics();
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
        CameraSystem.cameraSystem.ActivateShake(2, 0.1f);
    }
}
