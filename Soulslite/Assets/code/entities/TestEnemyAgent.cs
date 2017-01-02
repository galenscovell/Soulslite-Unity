﻿using System.Collections;
using UnityEngine;


public class TestEnemyAgent : Enemy
{
    private SeekBehavior behavior;
    private Seeker seeker;

    private TestenemyAttackState attackState;
    private int attackStateHash = Animator.StringToHash("Base Layer.TestenemyAttackState");

    private TestenemyHurtState hurtState;
    private int hurtStateHash = Animator.StringToHash("Base Layer.TestenemyHurtState");


    /**************************
     *          Init          *
     **************************/
    private new void Start() 
    {
        base.Start();

        behavior = new SeekBehavior();
        seeker = GetComponent<Seeker>();

        attackState = animator.GetBehaviour<TestenemyAttackState>();
        attackState.Setup(this);

        hurtState = animator.GetBehaviour<TestenemyHurtState>();
        hurtState.Setup(this);
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
        /*****************
         * PASSIVE
         *****************/
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
        /*****************
         * ACTIVE
         *****************/
        else
        {
            // Check if attack ready
            bool attackReady = AttackCheck();

            currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (currentStateInfo.fullPathHash != attackStateHash)
            {
                // Repath
                if (repathCounter <= 0)
                {
                    repathCounter = repathRate;
                    behavior.SetPath(seeker, body.position, TrackTarget());
                }

                // Follow set path
                if (behavior.HasPath())
                {
                    // Target in range, view, and attack ready
                    if (InAttackRange() && TargetInView() && attackReady)
                    {
                        animator.SetBool("Attacking", true);
                    }
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
                // No path to follow, stop motion
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
            if (currentStateInfo.fullPathHash == attackStateHash && !attackState.Interrupt(animator))
            {
                return;
            }

            if (currentStateInfo.fullPathHash == hurtStateHash)
            {
                return;
            }

            hurtState.SetHurtVelocity(collision.attachedRigidbody.velocity.normalized);
            Hurt();
        }
    }


    /**************************
     *          Hurt          *
     **************************/
    private void Hurt()
    {
        animator.Play("TestenemyHurtState");
        StartCoroutine(HurtFlash());
    }
}
