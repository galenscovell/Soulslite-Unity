using System.Collections.Generic;
using UnityEngine;


public class BulletSystem : MonoBehaviour
{
    public GameObject bulletObject;
    public GameObject bulletCollisionObject;

    private List<GameObject> bullets;
    private List<GameObject> bulletCollisions;

    private int maxBullets = 20;
    private int spawnedBullets;
    private int bulletObjectIndex;
    private int bulletCollisionObjectIndex;


    private void Start()
    {
        // Create two object pools for bullets and bullet collisions
        bullets = new List<GameObject>();
        bulletCollisions = new List<GameObject>();

        for (int i = 0; i < maxBullets; i++)
        {
            GameObject bulletObj = Instantiate(bulletObject);
            bulletObj.SetActive(false);
            bulletObj.transform.parent = transform;
            bullets.Add(bulletObj);

            GameObject bulletCollisionObj = Instantiate(bulletCollisionObject);
            bulletCollisionObj.SetActive(false);
            bulletCollisionObj.transform.parent = transform;
            bulletCollisions.Add(bulletCollisionObj);
        }

        bulletObjectIndex = 0;
        bulletCollisionObjectIndex = 0;
    }
}
