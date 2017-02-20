using System.Collections.Generic;
using UnityEngine;


public class EnemyRangedGunLimb : MonoBehaviour
{
    public static EnemyRangedGunLimb enemyRangedGunLimb;

    public List<Sprite> sprites;

    private SpriteRenderer spriteRenderer;
    private int currentSprite = 0;
    private float gunAngle;

    private float lerpTime = 1f;
    private float currentLerpTime = 0;
    private Quaternion rotationQuaternion;
    private bool rotating = false;


    private void Awake()
    {
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

    private void Update()
    {
        if (rotating)
        {
            if (currentLerpTime < lerpTime)
            {
                currentLerpTime += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(transform.rotation, rotationQuaternion, currentLerpTime / lerpTime);
            }
            else
            {
                rotating = false;
                currentLerpTime = 0;
            }
        }
    }

    public void UpdateGunLimb(Vector2 vectorDiff, Enemy enemy)
    {
        if (enemy.GetFacingDirection().x > 0)
        {
            transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
        else
        {
            transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
        }

        float angle = Mathf.Atan2(vectorDiff.y, vectorDiff.x) * Mathf.Rad2Deg;
        rotationQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);
        rotating = true;

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

    public void ResetGunLimb(Enemy enemy)
    {
        float angle;
        if (enemy.GetFacingDirection().x > 0)
        {
            angle = 0;
        }
        else
        {
            angle = 180;
        }

        rotationQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);
        rotating = true;
    }

    public Vector3 GetBarrelPosition()
    {
        // This is a hack -- ideally let's make the sprite in such a way to not require
        // such extra fine tuned offsetting of the gun barrel
        float multiplier = 15f;

        // This is the primary offsetting which we will hopefully be able to use
        // alone in the future
        Vector3 rightTransform = transform.right * multiplier;
        Vector3 barrelPosition = new Vector3(
            gameObject.transform.position.x + rightTransform.x,
            gameObject.transform.position.y + rightTransform.y,
            gameObject.transform.position.z
        );
        return barrelPosition;
    }

    private void UpdateSprite()
    {
        // Right
        if (currentSprite != 0 && (gunAngle >= 0 && gunAngle < 180))
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
