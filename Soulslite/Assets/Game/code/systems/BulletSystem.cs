using System.Collections.Generic;
using UnityEngine;


public class BulletSystem : MonoBehaviour
{
    public static BulletSystem bulletSystem;

    public GameObject bulletFireObject;
    public GameObject bulletObject;
    public GameObject bulletCollisionObject;

    private List<GameObject> bulletFires;
    private List<GameObject> bullets;
    private List<GameObject> bulletCollisions;

    private int maxBullets = 20;
    private int spawnedBullets;
    private int bulletFireIndex;
    private int bulletObjectIndex;
    private int bulletCollisionObjectIndex;

    private Vector2 nextBulletDirection;


    private void Awake()
    {
        if (bulletSystem != null) Destroy(bulletSystem);
        else bulletSystem = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // Create separate object pools for fires, bullets and collisions
        bulletFires = new List<GameObject>();
        bullets = new List<GameObject>();
        bulletCollisions = new List<GameObject>();

        for (int i = 0; i < maxBullets; i++)
        {
            GameObject fireObj = Instantiate(bulletFireObject);
            fireObj.SetActive(false);
            fireObj.transform.parent = transform;
            bulletFires.Add(fireObj);

            GameObject bulletObj = Instantiate(bulletObject);
            bulletObj.SetActive(false);
            bulletObj.transform.parent = transform;
            bullets.Add(bulletObj);

            GameObject bulletCollisionObj = Instantiate(bulletCollisionObject);
            bulletCollisionObj.SetActive(false);
            bulletCollisionObj.transform.parent = transform;
            bulletCollisions.Add(bulletCollisionObj);
        }

        bulletFireIndex = 0;
        bulletObjectIndex = 0;
        bulletCollisionObjectIndex = 0;
    }

    public Vector2 GetNextBulletDirection()
    {
        return nextBulletDirection;
    }

    public void SpawnBullet(Vector2 position, Vector2 direction, string designatedTag, string designatedLayer)
    {
        nextBulletDirection = direction;

        // Ensure object indices are within pool size
        if (bulletFireIndex >= maxBullets) bulletFireIndex = 0;
        if (bulletObjectIndex >= maxBullets) bulletObjectIndex = 0;

        // Pull out bullet fire particle effect at spawn location
        // Also rotate it to face in firing direction
        GameObject fireObj = bulletFires[bulletFireIndex];
        fireObj.transform.rotation = Quaternion.LookRotation(direction);
        fireObj.transform.position = position;
        fireObj.SetActive(true);

        // Pull out a bullet object and mark it active
        // Also set its tag for collision layers and set it to spawn location
        GameObject bulletObj = bullets[bulletObjectIndex];
        bulletObj.tag = designatedTag;
        bulletObj.layer = LayerMask.NameToLayer(designatedLayer);
        bulletObj.transform.position = position;
        bulletObj.transform.right = direction;
        bulletObj.SetActive(true);

        spawnedBullets++;
        bulletFireIndex++;
        bulletObjectIndex++;
    }

    public void DespawnBullet(GameObject gameObj, Vector2 collisionDirection)
    {
        // Disable bullet
        gameObj.SetActive(false);
        spawnedBullets--;

        // Ensure particle index is within pool size
        if (bulletCollisionObjectIndex >= maxBullets) bulletCollisionObjectIndex = 0;

        // Enable particle system at bullets last position
        GameObject bulletCollisionParticle = bulletCollisions[bulletCollisionObjectIndex];

        // Rotate particles opposite collision direction and set it to target position
        bulletCollisionParticle.transform.rotation = Quaternion.LookRotation(collisionDirection);
        bulletCollisionParticle.transform.position = gameObj.transform.position;
        bulletCollisionParticle.SetActive(true);

        bulletCollisionObjectIndex++;
    }

    public void DespawnParticle(GameObject gameObj)
    {
        // Disable particle system
        gameObj.SetActive(false);
    }
}
