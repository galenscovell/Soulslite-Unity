using System.Collections.Generic;
using UnityEngine;


public class RainSystem : MonoBehaviour
{
    public static RainSystem rainSystem;

    public new Camera camera;
    public GameObject rainSplashObject;
    public GameObject rainDropObject;
    public int maxRaindrops;
    public int maxSplashes;
    public int rainRate;

    private List<GameObject> rainDrops;
    private List<GameObject> rainSplashes;

    private float minX, maxX, minY, maxY;
    private int spawnedRaindrops;
    private int dropObjectIndex;
    private int splashObjectIndex;


    private void Awake()
    {
        // Singleton, kept between scenes
        if (rainSystem != null) Destroy(rainSystem);
        else rainSystem = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // Create two object pools for raindrops and particle systems
        rainDrops = new List<GameObject>();
        rainSplashes = new List<GameObject>();

        for (int i = 0; i < maxRaindrops; i++)
        {
            GameObject dropObj = Instantiate(rainDropObject);
            dropObj.SetActive(false);
            dropObj.transform.parent = transform;
            rainDrops.Add(dropObj);
        }

        // We need more particle objects than drops since they have longer lifespans
        for (int i = 0; i < maxSplashes; i++)
        {
            GameObject particleObj = Instantiate(rainSplashObject);
            particleObj.SetActive(false);
            particleObj.transform.parent = transform;
            rainSplashes.Add(particleObj);
        }

        dropObjectIndex = 0;
        splashObjectIndex = 0;
    }

	private void Update()
    {
        UpdateCameraBounds();

        // Pull out up to rainRate drop objects from pool if max number of drops isn't reached
        for (var x = 0; x < rainRate; x++)
        {
            if (spawnedRaindrops < maxRaindrops)
            {
                // Find random x and y within (and slightly outside of) camera bounds
                Vector2 position = new Vector2(
                    Random.Range(minX, maxX),
                    Random.Range(minY, maxY)
                );

                SpawnRaindrop(position);
            }
        }
    }

    private void SpawnRaindrop(Vector2 position)
    {
        // Ensure object index is within pool size
        if (dropObjectIndex >= maxRaindrops) dropObjectIndex = 0;

        // Pull out a raindrop object, set it to spawn location, and mark it active
        GameObject nextDrop = rainDrops[dropObjectIndex];
        nextDrop.transform.position = position;
        nextDrop.SetActive(true);

        spawnedRaindrops++;
        dropObjectIndex++;
    }

    public void DespawnRaindrop(GameObject gameObj)
    {
        // Disable raindrop
        gameObj.SetActive(false);
        spawnedRaindrops--;

        // Ensure particle index is within pool size
        if (splashObjectIndex >= maxSplashes) splashObjectIndex = 0;

        // Enable particle system at raindrops last position
        GameObject rainParticle = rainSplashes[splashObjectIndex];
        rainParticle.transform.position = gameObj.transform.position;
        rainParticle.SetActive(true);

        splashObjectIndex++;
    }

    public void DespawnParticle(GameObject gameObj)
    {
        // Disable particle system
        gameObj.SetActive(false);
    }

    private void UpdateCameraBounds()
    {
        // Find cameras bottom left and top right corners as Vector2s
        // These have give so drops can spawn slightly outside of camera
        Vector2 cameraBottomLeft = camera.ViewportToWorldPoint(new Vector2(-0.25f, -0.25f));
        Vector2 cameraTopRight = camera.ViewportToWorldPoint(new Vector2(1.25f, 1.25f));

        // Establish x bounds
        minX = cameraBottomLeft.x;
        maxX = cameraTopRight.x;

        // Establish y bounds
        minY = cameraBottomLeft.y;
        maxY = cameraTopRight.y;
    }
}
