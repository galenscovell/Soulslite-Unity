using System.Collections;
using UnityEngine;


public class MinibossJump : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.MinibossJump");
    private Enemy enemy;

    private int[] sfx;

    private Vector2 startPosition;
    private Vector2 jumpTarget;
    private bool jumped;
    private bool jumpSfx;
    private bool landed;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e, int[] assignedSfx)
    {
        enemy = e;
        sfx = assignedSfx;
    }

    public bool SetJumpTarget(Vector2 target)
    {
        startPosition = enemy.transform.position;
        jumpTarget = target + new Vector2(0, 20);

        var distanceToTarget = (jumpTarget - startPosition).magnitude;
        if (distanceToTarget < 200)
        {
            return true;
        }

        return false;
    }



    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        jumped = false;
        jumpSfx = false;
        landed = false;

        enemy.IgnoreAllCollisions();
        enemy.DisableMotion();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.3f && stateTime < 0.35f)
        {
            if (!jumped)
            {
                enemy.StartCoroutine(Jump(200, 0.75f));
                jumped = true;
            }
        }
        else if (stateTime > 0.35f && stateTime < 0.7f)
        {
            if (!jumpSfx)
            {
                enemy.SetSortingLayer("Foreground");
                enemy.PlaySfxRandomPitch(sfx[0], 0.8f, 1.1f, 0.4f);
                jumpSfx = true;
            }
        }
        else if (stateTime > 0.7f && stateTime < 1f)
        {
            if (!landed)
            {
                enemy.SetSortingLayer("Entity");
                enemy.RestoreCollisions();
                enemy.PlaySfxRandomPitch(sfx[1], 0.8f, 1.1f, 0.4f);
                DustSystem.dustSystem.SpawnShockDust(enemy.GetBody().position);
                CameraSystem.cameraSystem.ActivateShake(4, 0.3f);
                landed = true;
            }
        }
        else if (stateTime > 1f)
        {
            animator.SetBool("Jumping", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableMotion();
    }



    private IEnumerator Jump(float height, float time)
    {
        var startPos = enemy.transform.position;
        var timer = 0.0f;

        // Debug.DrawLine(startPos, jumpTarget, Color.red, 1f);

        while (timer < 1)
        {
            timer += Time.deltaTime / time;

            Vector3 parabolaPoint = SampleParabola(startPos, jumpTarget, height, timer);
            // Debug.DrawLine(enemy.transform.position, parabolaPoint, Color.green, timer);
            enemy.transform.position = Vector3.Lerp(startPos, parabolaPoint, timer);

            yield return null;
        }
    }

    private Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            // start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            // start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross(right, levelDirecteion);

            if (end.y > start.y)
            {
                up = -up;
            }

            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }
}