using UnityEngine;


public class PlayerRangedReady : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerRanged.PlayerRangedReady");
    private PlayerAgent player;

    private RaycastHit2D hit;
    private float laserRange = 200f;
    private LineRenderer laser;
    private int laserMask = ~(1 << 10);


    public int GetHash()
    {
        return hash;
    }

    public void Setup(PlayerAgent playerEntity, LineRenderer line)
    {
        player = playerEntity;
        laser = line;
        laser.sortingLayerName = "Foreground";
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 gunBarrel = player.GetPlayerGunBarrel();
        Vector2 targetLocation = gunBarrel + (player.facingDirection.normalized * laserRange);

        hit = Physics2D.Linecast(gunBarrel, targetLocation, laserMask);
        if (hit)
        {
            laser.SetPosition(0, gunBarrel);
            laser.SetPosition(1, hit.point);
        }
        else
        {
            laser.SetPosition(0, gunBarrel);
            laser.SetPosition(1, targetLocation);
        }
        laser.enabled = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 gunBarrel = player.GetPlayerGunBarrel();
        Vector2 targetLocation = gunBarrel + (player.facingDirection.normalized * laserRange);

        hit = Physics2D.Linecast(gunBarrel, targetLocation, laserMask);
        if (hit)
        {
            laser.SetPosition(0, gunBarrel);
            laser.SetPosition(1, hit.point);
        }
        else
        {
            laser.SetPosition(0, gunBarrel);
            laser.SetPosition(1, targetLocation);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        laser.enabled = false;
    }
}
