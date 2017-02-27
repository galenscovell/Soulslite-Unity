using UnityEngine;


public class EnemyFall : StateMachineBehaviour
{
    public string StateName;

    private int hash;
    private Enemy enemy;
    private int sfxIndex;

    private bool descentBegan;
    private bool fadeOutBegan;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e, int assignedSfxIndex)
    {
        hash = Animator.StringToHash(StateName);
        enemy = e;
        sfxIndex = assignedSfxIndex;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        descentBegan = false;
        fadeOutBegan = false;

        enemy.IgnoreAllPhysics();
        enemy.FaceTarget();
        enemy.SetHurtImpulse(new Vector2(enemy.GetFacingDirection().x * -0.5f, -0.5f), 4, 0.15f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.1f && stateTime < 0.15f)
        {
            if (!descentBegan)
            {
                float facingX = enemy.GetFacingDirection().x;
                enemy.SetHurtImpulse(new Vector2(facingX * -0.05f, -1), 3, 0.5f);
                descentBegan = true;
            }

            if (!fadeOutBegan)
            {
                enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.1f, 0.5f);
                enemy.FadeOutSprite(0.3f);
                fadeOutBegan = true;
            }
        }
        else if (stateTime > 1)
        {
            enemy.Disable();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}

