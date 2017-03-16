using UnityEngine;


public class MinibossEntrance : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.MinibossEntrance");
    private Enemy enemy;

    private int[] sfx;

    private bool landed;
    private bool roared;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e, int[] assignedSfx)
    {
        enemy = e;
        sfx = assignedSfx;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        landed = false;
        roared = false;

        enemy.IgnoreAllCollisions();
        enemy.SetSpeed(460);
        enemy.SetNextVelocity(new Vector2(0, -1));
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.25f && stateTime < 0.3f)
        {
            if (!landed)
            {
                LevelSystem.levelSystem.ChangeMusic(1);
                CameraSystem.cameraSystem.ChangeTarget(enemy.gameObject);
                enemy.SetNextVelocity(Vector2.zero);
                enemy.RestoreDefaultSpeed();
                enemy.PlaySfx(sfx[0], 0.8f, 1f);
                DustSystem.dustSystem.SpawnShockDust(enemy.GetBody().position);
                CameraSystem.cameraSystem.ActivateShake(4, 0.3f);

                landed = true;
            }
        }
        else if (stateTime > 0.6f && stateTime < 0.65f)
        {
            if (!roared)
            {
                enemy.PlaySfx(sfx[1], 0.5f, 0.8f);
                CameraSystem.cameraSystem.ActivateShake(3, 0.7f);

                roared = true;
            }
        }
        else if (stateTime > 1)
        {

        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CameraSystem.cameraSystem.ChangeTarget(LevelSystem.levelSystem.player.gameObject);
        enemy.RestoreCollisions();
    }
}