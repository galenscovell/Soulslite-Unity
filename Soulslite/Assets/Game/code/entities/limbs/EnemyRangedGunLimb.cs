using System.Collections.Generic;
using UnityEngine;


public class EnemyRangedGunLimb : MonoBehaviour
{
    public static EnemyRangedGunLimb enemyRangedGunLimb;

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

    public void UpdateGunLimb(Vector2 vectorDiff)
    {
        float angle = Mathf.Atan2(vectorDiff.y, vectorDiff.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (vectorDiff.x < 0)
        {
            gunAngle = 360 - (Mathf.Atan2(vectorDiff.x, vectorDiff.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            gunAngle = Mathf.Atan2(vectorDiff.x, vectorDiff.y) * Mathf.Rad2Deg;
        }

        UpdateSprite();
    }

    public Vector3 GetBarrelPosition()
    {
        // This is a hack -- ideally let's make the sprite in such a way to not require
        // such extra fine tuned offsetting of the gun barrel
        float multiplier = 1;
        float addY = 0;
        if (currentSprite == 0)
        {
            
        }
        else if (currentSprite == 1)
        {
            
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

    private void UpdateSprite()
    {
        // Right
        if (currentSprite != 0 && (gunAngle >= 0 && gunAngle <= 180))
        {
            currentSprite = 0;
            spriteRenderer.flipY = false;
            spriteRenderer.sprite = sprites[currentSprite];
        }
        // Left
        else if (currentSprite != 1 && (gunAngle >= 180 && gunAngle <= 360))
        {
            currentSprite = 1;
            spriteRenderer.flipY = true;
            spriteRenderer.sprite = sprites[currentSprite];
        }
    }
}
