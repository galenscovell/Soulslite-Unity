using UnityEngine;


public class MinibossRoar : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.MinibossRoar");
    private Enemy enemy;

    private int sfxIndex;

    private bool roared;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e, int assignedSfx)
    {
        enemy = e;
        sfxIndex = assignedSfx;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        roared = false;

        enemy.FaceTarget();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.25f && stateTime < 0.3f)
        {
            if (!roared)
            {
                enemy.PlaySfx(sfxIndex, 0.5f, 0.8f);
                CameraSystem.cameraSystem.ActivateShake(3, 0.7f);

                roared = true;
            }
        }
        else if (stateTime > 1)
        {
            animator.SetBool("Roaring", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}