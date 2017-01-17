using UnityEngine;


public class PlayerRanged : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerRanged.PlayerRanged");
    private PlayerAgent player;
    private PlayerGunLimb gunLimb;
    private int sfxIndex;

    private RaycastHit2D hit;
    private float laserRange = 200f;
    private LineRenderer laser;
    private int laserMask = ~(1 << 10);

    public int GetHash()
    {
        return hash;
    }

    public void LoadGun()
    {
        player.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
    }

    public void Setup(PlayerAgent playerEntity, PlayerGunLimb gun, LineRenderer line, int assignedSfxIndex)
    {
        player = playerEntity;
        gunLimb = gun;
        laser = line;
        laser.sortingLayerName = "Foreground";
        sfxIndex = assignedSfxIndex;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.DisableMotion();
        gunLimb.Activate();

        gunLimb.UpdateTransform(player.facingDirection);
        UpdateLasersight();

        laser.enabled = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gunLimb.UpdateTransform(player.facingDirection);
        UpdateLasersight();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        laser.enabled = false;
        gunLimb.Deactivate();
        player.EnableMotion();
    }

    private void UpdateLasersight()
    {
        Vector2 gunBarrel = gunLimb.transform.position;
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
}
