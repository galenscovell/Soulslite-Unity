using System.Collections.Generic;
using UnityEngine;


public class DustSystem : MonoBehaviour
{
    public static DustSystem dustSystem;

    public GameObject dustObject;

    private List<GameObject> dustObjects;

    private int maxDust = 20;
    private int spawnedDust;
    private int dustObjectIndex;

    private float nextDustForce;


    private void Awake()
    {
        // Singleton, kept between scenes
        if (dustSystem != null) Destroy(dustSystem);
        else dustSystem = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // Create object pools for dust objects
        dustObjects = new List<GameObject>();

        for (int i = 0; i < maxDust; i++)
        {
            GameObject dustObj = Instantiate(dustObject);
            dustObj.SetActive(false);
            dustObj.transform.parent = transform;
            dustObjects.Add(dustObj);
        }

        dustObjectIndex = 0;
    }

    public float GetNextDustForce()
    {
        return nextDustForce;
    }

    public void SpawnDust(Vector2 rawPosition, Vector2 facingDirection)
    {
        nextDustForce = facingDirection.magnitude;

        if (dustObjectIndex >= maxDust) dustObjectIndex = 0;

        float x = rawPosition.x - (facingDirection.x * 14);
        float y = rawPosition.y;
        if (facingDirection.y == 0)
        {
            y -= 14;
        }
        else
        {
            y -= (facingDirection.y * 16);
        }

        Vector2 stepPosition = new Vector2(x, y);

        GameObject dustObj = dustObjects[dustObjectIndex];
        // dustObj.transform.rotation = Quaternion.LookRotation(facingDirection);
        dustObj.transform.position = stepPosition;
        dustObj.SetActive(true);

        spawnedDust++;
    }

    public void DespawnDust(GameObject gameObj)
    {
        gameObj.SetActive(false);
        spawnedDust--;
    }
}
