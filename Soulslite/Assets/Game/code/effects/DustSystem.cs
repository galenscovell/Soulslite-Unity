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
        // Singleton, destroyed between scenes
        if (dustSystem != null) Destroy(dustSystem);
        else dustSystem = this;
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
        // TODO: Use force to change size of dust produced by movement
        // Force is float between 1-8

        if (dustObjectIndex >= maxDust) dustObjectIndex = 0;

        float x = rawPosition.x;
        float y = rawPosition.y;
        if (facingDirection.y == 0)
        {
            x -= facingDirection.x * 12;
            y -= 14;
        }
        else
        {
            if (facingDirection.y < 0)
            {
                x -= facingDirection.x * 20;
                y -= facingDirection.y * 8;
            }
            else
            {
                x -= facingDirection.x * 18;
                y -= facingDirection.y * 20;
            }
        }

        Vector2 stepPosition = new Vector2(x, y);

        GameObject dustObj = dustObjects[dustObjectIndex];
        dustObj.transform.position = stepPosition;
        dustObj.SetActive(true);

        dustObjectIndex++;
        spawnedDust++;
    }

    public void DespawnDust(GameObject gameObj)
    {
        gameObj.SetActive(false);
        spawnedDust--;
    }
}
