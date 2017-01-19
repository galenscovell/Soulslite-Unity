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
        if (currentSprite != 0 && AngleWithinOctants(1, 2, true, true))
        {
            currentSprite = 0;
            spriteRenderer.sprite = sprites[0];
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
            spriteRenderer.sortingLayerName = "Foreground";
        }
        // Down
        else if (currentSprite != 1 && AngleWithinOctants(3, 4, true, true))
        {
            currentSprite = 1;
            spriteRenderer.sprite = sprites[1];
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
            spriteRenderer.sortingLayerName = "Foreground";
        }
        // Left
        else if (currentSprite != 2 && AngleWithinOctants(5, 6, true, true))
        {
            currentSprite = 2;
            spriteRenderer.sprite = sprites[2];
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = true;
            spriteRenderer.sortingLayerName = "Foreground";
        }
        // Up
        else if (currentSprite != 3 && (AngleWithinOctants(7, 8, true, true) || AngleWithinOctants(0, 1, true, false)))
        {
            currentSprite = 3;
            spriteRenderer.sprite = sprites[3];
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = true;
            spriteRenderer.sortingLayerName = "Entity";
        }
    }

    private int GetAngleOctant()
    {
        // The octant is the space between the values below
        // Eg Octant '0' is between 0 and 1 (or angles between 0 and 45)
        //    0
        //  7   1
        //6       2
        //  5   3
        //    4
        if (gunAngle >= 0 && gunAngle < 45) return 0;
        else if (gunAngle >= 45 && gunAngle < 90) return 1;
        else if (gunAngle >= 90 && gunAngle < 135) return 2;
        else if (gunAngle >= 135 && gunAngle < 180) return 3;
        else if (gunAngle >= 180 && gunAngle < 215) return 4;
        else if (gunAngle >= 215 && gunAngle < 270) return 5;
        else if (gunAngle >= 270 && gunAngle < 315) return 6;
        else if (gunAngle >= 315 && gunAngle < 360) return 7;
        return 0;
    }

    private bool AngleWithinOctants(int startOctant, int endOctant, bool startInclusive, bool endInclusive)
    {
        int octant = GetAngleOctant();
        bool withinStart = false;
        bool withinEnd = false;

        if (startInclusive) withinStart = octant >= startOctant;
        else withinStart = octant > startOctant;

        if (endInclusive) withinEnd = octant <= endOctant;
        else withinEnd = octant < endOctant;

        return withinStart && withinEnd;
    }
}
