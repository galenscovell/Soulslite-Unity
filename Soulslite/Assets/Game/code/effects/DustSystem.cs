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

    private Vector2 nextDustDirection;


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

    public Vector2 GetNextDustDirection()
    {
        return nextDustDirection;
    }

    public void SpawnDust(Vector2 position, Vector2 direction)
    {
        nextDustDirection = direction;

        if (dustObjectIndex >= maxDust) dustObjectIndex = 0;

        GameObject dustObj = dustObjects[dustObjectIndex];
        dustObj.transform.position = position;
        dustObj.SetActive(true);

        spawnedDust++;
    }

    public void DespawnDust(GameObject gameObj, Vector2 collisionDirection)
    {
        // Disable bullet
        gameObj.SetActive(false);
        spawnedDust--;
    }
}
