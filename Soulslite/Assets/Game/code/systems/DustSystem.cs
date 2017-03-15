using System.Collections.Generic;
using UnityEngine;


public class DustSystem : MonoBehaviour
{
    public static DustSystem dustSystem;

    public DustObject dustObject;
    public ShockDustObject shockDustObject;

    private List<DustObject> dustObjects;
    private List<ShockDustObject> shockDustObjects;

    private int maxDust = 20;
    private int maxShockDust = 6;
    private int spawnedDust;
    private int dustObjectIndex;
    private int shockDustObjectIndex;


    private void Awake()
    {
        if (dustSystem != null) Destroy(dustSystem);
        else dustSystem = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // Create object pools for dust objects
        dustObjects = new List<DustObject>();

        for (int i = 0; i < maxDust; i++)
        {
            DustObject dustObj = Instantiate(dustObject);
            dustObj.gameObject.SetActive(false);
            dustObj.transform.parent = transform;
            dustObjects.Add(dustObj);
        }

        // Create object pools for shock dust objects
        shockDustObjects = new List<ShockDustObject>();

        for (int i = 0; i < maxShockDust; i++)
        {
            ShockDustObject shockDustObj = Instantiate(shockDustObject);
            shockDustObj.gameObject.SetActive(false);
            shockDustObj.transform.parent = transform;
            shockDustObjects.Add(shockDustObj);
        }

        dustObjectIndex = 0;
        shockDustObjectIndex = 0;
    }

    public void SpawnDust(Vector2 rawPosition, Vector2 facingDirection)
    {
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

        DustObject dustObj = dustObjects[dustObjectIndex];
        dustObj.transform.position = stepPosition;
        dustObj.gameObject.SetActive(true);

        dustObjectIndex++;
        spawnedDust++;
    }

    public void DespawnDust(DustObject dustObj)
    {
        dustObj.gameObject.SetActive(false);
        spawnedDust--;
    }

    public void SpawnShockDust(Vector2 rawPosition)
    {
        if (shockDustObjectIndex >= maxShockDust) shockDustObjectIndex = 0;

        ShockDustObject shockDustObj = shockDustObjects[shockDustObjectIndex];
        shockDustObj.transform.position = new Vector2(rawPosition.x, rawPosition.y - 20);
        shockDustObj.gameObject.SetActive(true);

        shockDustObjectIndex++;
    }

    public void DespawnShockDust(ShockDustObject shockDustObj)
    {
        shockDustObj.gameObject.SetActive(false);
    }
}
