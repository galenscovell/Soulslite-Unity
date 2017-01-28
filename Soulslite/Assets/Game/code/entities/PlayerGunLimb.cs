using System.Collections.Generic;
using UnityEngine;


public class PlayerGunLimb : MonoBehaviour
{
    public static PlayerGunLimb playerGunLimb;

    public List<Sprite> sprites;

    private Collider2D gunCollider;
    private SpriteRenderer spriteRenderer;
    private int currentSprite = 0;
    private float gunAngle;


    private void Awake()
    {
        gunCollider = GetComponent<Collider2D>();
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
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;

        gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        UpdateSprite(normalizedPlayerFacing);
    }

    public Vector3 GetBarrelPosition()
    {
        // This is a hack -- ideally let's make the sprite in such a way to not require
        // such extra fine tuned offsetting of the gun barrel
        float multiplier = 1;
        float addY = 0;
        if (currentSprite == 0)
        {
            multiplier = 15f;
            addY = 2.5f;
        }
        else if (currentSprite == 1)
        {
            multiplier = 16;
            addY = 0;
        }
        else if (currentSprite == 2)
        {
            multiplier = 15f;
            addY = 2.5f;
        }
        else if (currentSprite == 3)
        {
            multiplier = 14.5f;
            addY = 0;
        }

        // This is the primary offsetting which we will hopefully be able to use
        // alone in the future
        Vector3 rightTransform = transform.right * multiplier;
        Vector3 barrelPosition = new Vector3(
            gameObject.transform.position.x + rightTransform.x,
            gameObject.transform.position.y + rightTransform.y + addY,
            gameObject.transform.position.z
        );
        return barrelPosition;
    }

    private void UpdateSprite(Vector2 facingDirection)
    {
        if (facingDirection.x < 0)
        {
            gunAngle = 360 - (Mathf.Atan2(facingDirection.x, facingDirection.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            gunAngle = Mathf.Atan2(facingDirection.x, facingDirection.y) * Mathf.Rad2Deg;
        }

        // Right
        if (currentSprite != 0 && (gunAngle >= 30 && gunAngle <= 150))
        {
            currentSprite = 0;
            spriteRenderer.sprite = sprites[0];
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
            spriteRenderer.sortingLayerName = "Foreground";
        }
        // Down
        else if (currentSprite != 1 && (gunAngle > 150 && gunAngle < 210))
        {
            currentSprite = 1;
            spriteRenderer.sprite = sprites[1];
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
            spriteRenderer.sortingLayerName = "Foreground";
        }
        // Left
        else if (currentSprite != 2 && (gunAngle >= 210 && gunAngle <= 330))
        {
            currentSprite = 2;
            spriteRenderer.sprite = sprites[2];
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = true;
            spriteRenderer.sortingLayerName = "Foreground";
        }
        // Up
        if (currentSprite != 3 && (gunAngle > 330 || gunAngle < 30))
        {
            currentSprite = 3;
            spriteRenderer.sprite = sprites[3];
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = true;
            spriteRenderer.sortingLayerName = "Entity";
        }
    }
}
