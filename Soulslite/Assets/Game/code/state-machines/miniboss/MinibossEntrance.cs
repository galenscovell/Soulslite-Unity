using UnityEngine;


public class MinibossEntrance : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.MinibossEntrance");
    private MinibossAgent enemy;

    private int[] sfx;

    private bool landed;
    private bool roared;

    private BlobShadow jumpShadow;


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

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        landed = false;
        roared = false;

        enemy.GetShadow().TurnOff();

        LevelSystem.levelSystem.player.SetNextVelocity(Vector2.zero);
        LevelSystem.levelSystem.player.EnableInput(false);

        // Small to large over time to simulate enemy getting closer to ground
        jumpShadow.TurnOn();
        jumpShadow.LerpScale(new Vector2(0, 0), 0);
        jumpShadow.LerpScale(new Vector2(3, 2), 0.4f);

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
                jumpShadow.TurnOff();
                enemy.GetShadow().TurnOn();

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
        LevelSystem.levelSystem.player.EnableInput(true);

        enemy.RestoreCollisions();

        UISystem.uiSystem.EnableBossHealthDisplay(true);
        UISystem.uiSystem.UpdateBossHealth(20);
    }
}