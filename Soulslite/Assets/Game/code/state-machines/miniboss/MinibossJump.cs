using System.Collections;
using UnityEngine;


public class MinibossJump : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.MinibossJump");
    private MinibossAgent enemy;

    private int[] sfx;

    private Vector2 startPosition;
    private Vector2 jumpTarget;
    private bool jumped;
    private bool jumpSfx;
    private bool landed;

    private BlobShadow jumpShadow;
    private Vector2 shadowOffset = new Vector2(-2, -35);


    public int GetHash()
    {
        return hash;
    }

    public void Setup(MinibossAgent e, int[] assignedSfx, BlobShadow shadow)
    {
        enemy = e;
        sfx = assignedSfx;
        jumpShadow = shadow;
    }

    public bool CheckTargetRange(Vector2 target)
    {
        startPosition = enemy.transform.position;
        jumpTarget = target;

        var distanceToTarget = (jumpTarget - startPosition).magnitude;
        if (distanceToTarget < 180)
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
        
        jumpShadow.SetWorldPosition(enemy.transform.position + new Vector3(-2, -25, 1));
        enemy.GetShadow().TurnOff();
        jumpShadow.TurnOn();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.3f && stateTime < 0.35f)
        {
            if (!jumped)
            {
                jumpTarget = enemy.TrackTarget() + new Vector2(0, 32);
                jumpShadow.LerpWorldPosition(jumpTarget + shadowOffset, 0.8f);
                jumpShadow.LerpScaleInThenOut(new Vector2(3, 2), new Vector2(1, 0.5f), 0.8f);
                enemy.StartCoroutine(Jump(240, 0.8f));
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
                jumpShadow.TurnOff();
                enemy.GetShadow().TurnOn();

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

        while (timer < 1)
        {
            timer += Time.deltaTime / time;

            Vector3 parabolaPoint = SampleParabola(startPos, jumpTarget, height, timer);
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