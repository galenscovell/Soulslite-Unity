using UnityEngine;


public class EnemyRangedFall : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.EnemyRangedFall");
    private Enemy enemy;
    private int sfxIndex;

    private bool fadeOutBegan;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e, int assignedSfxIndex)
    {
        enemy = e;
        sfxIndex = assignedSfxIndex;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.IgnoreAllPhysics();
        fadeOutBegan = false;
        enemy.FaceTarget();
        float facingX = enemy.GetFacingDirection().x;
        enemy.SetHurtImpulse(new Vector2(facingX * -0.5f, -0.5f), 4, 0.15f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (stateTime > 0.1f && stateTime < 0.15f)
        {
            if (!fadeOutBegan)
            {
                float facingX = enemy.GetFacingDirection().x;
                enemy.SetHurtImpulse(new Vector2(facingX * -0.05f, -1), 3, 0.5f);
                enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.1f, 0.5f);
                enemy.FadeOutSprite(0.25f);
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

