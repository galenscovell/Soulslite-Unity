using System.Collections.Generic;
using UnityEngine;


public class PlayerGunLimb : MonoBehaviour
{
    public static PlayerGunLimb playerGunLimb;
    private BoxCollider2D gunCollider;
    private SpriteRenderer spriteRenderer;

    public List<Sprite> sprites;
    private int currentSprite = 0;


    private void Awake()
    {
        gunCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void UpdateTransform(Vector2 facingDirection)
    {
        Vector2 normalizedPlayerFacing = facingDirection.normalized;
        float gunAngle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;

        gameObject.transform.rotation = Quaternion.AngleAxis(gunAngle, Vector3.forward);

        UpdateSprite(normalizedPlayerFacing);
    }

    public void UpdateSprite(Vector2 facingDirection)
    {
        float angle = 0f;
        if (facingDirection.x < 0)
        {
            angle = 360 - (Mathf.Atan2(facingDirection.x, facingDirection.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            angle = Mathf.Atan2(facingDirection.x, facingDirection.y) * Mathf.Rad2Deg;
        }

        if (angle >= 45 && angle <= 135)
        {
            if (currentSprite != 0)
            {
                currentSprite = 0;
                spriteRenderer.sprite = sprites[0];
                spriteRenderer.flipX = false;
                spriteRenderer.flipY = false;
                spriteRenderer.sortingLayerName = "Entity";
            }
        }
        else if (angle > 135 && angle < 225)
        {
            if (currentSprite != 1)
            {
                currentSprite = 1;
                spriteRenderer.sprite = sprites[1];
                spriteRenderer.flipX = false;
                spriteRenderer.flipY = false;
                spriteRenderer.sortingLayerName = "Foreground";
            }
        }
        else if (angle >= 225 && angle <= 315)
        {
            if (currentSprite != 2)
            {
                currentSprite = 2;
                spriteRenderer.sprite = sprites[2];
                spriteRenderer.flipX = true;
                spriteRenderer.flipY = true;
                spriteRenderer.sortingLayerName = "Entity";
            }
        }
        else if (angle > 315 && angle <= 360 || angle >= 0 && angle < 45)
        {
            if (currentSprite != 3)
            {
                currentSprite = 3;
                spriteRenderer.sprite = sprites[3];
                spriteRenderer.flipX = false;
                spriteRenderer.flipY = true;
                spriteRenderer.sortingLayerName = "Entity";
            }
        }
    }

    public Vector3 GetBarrelPosition()
    {
        return gunCollider.transform.position;
    }
}
