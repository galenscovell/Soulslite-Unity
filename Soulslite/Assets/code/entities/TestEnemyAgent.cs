using System.Collections;
using UnityEngine;


public class TestEnemyAgent : Enemy
{
    private SeekBehavior behavior;
    private Seeker seeker;

    private TestenemyAttackState attackState;
    private int attackStateHash = Animator.StringToHash("Base Layer.TestenemyAttackState");


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
                        animator.SetTrigger("Attack");
                    }
                    else
                    {
                        if (behavior.HasReachedWaypoint(body.position))
                        {
                            speedMultiplier = normalSpeed;
                            behavior.IncrementPath();
                            Vector2 dirToWaypoint = (behavior.GetNextWaypoint() - body.position).normalized;
                            nextVelocity = dirToWaypoint * speedMultiplier;
                        }
                    }
                }
                // No path to follow, stop motion
                else
                {
                    nextVelocity = Vector2.zero;
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
            StartCoroutine(Hurt());
        }
    }


    /**************************
     *          Hurt          *
     **************************/
    private IEnumerator Hurt()
    {
        //if (currentStateInfo.fullPathHash == attackStateHash) attackState.Interrupt(animator);
        //if (currentStateInfo.fullPathHash == dashStateHash) dashState.Interrupt(animator);

        //animator.Play("PlayerHurtState");

        spriteRenderer.material.SetFloat("_FlashAmount", 0.85f);
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.material.SetFloat("_FlashAmount", 0f);
    }
}
