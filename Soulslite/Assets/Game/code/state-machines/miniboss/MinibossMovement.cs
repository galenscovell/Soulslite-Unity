using System.Collections;
using UnityEngine;


public class MinibossMovement : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.MinibossMovement");
    private Enemy enemy;
    private int sfxIndex;
    private IEnumerator footstep;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e, int assignedSfxIndex)
    {
        enemy = e;
        sfxIndex = assignedSfxIndex;
        footstep = Footsteps();
    }

    private IEnumerator Footsteps()
    {
        while (true)
        {
            DustSystem.dustSystem.SpawnDust(enemy.GetBody().position, enemy.GetFacingDirection());
            yield return new WaitForSeconds(0.4f);
            enemy.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.2f, 0.5f);
            yield return new WaitForSeconds(0.4f);
        }
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.StartCoroutine(footstep);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.StopCoroutine(footstep);
    }
}
