using System.Collections.Generic;
using UnityEngine;


public class RainSystem : MonoBehaviour
{
    private int spawnedRaindrops;
    private float minX, maxX, minY, maxY;

    public new Camera camera;
    public GameObject rainDropParticle;
    public GameObject rainDropObject;
    public int maxRaindrops;

    private List<GameObject> rainDrops;
    private List<GameObject> rainParticles;

    private int dropObjectIndex;
    private int particleIndex;


    private void Start()
    {
        // Create two object pools for raindrops and particle systems
        rainDrops = new List<GameObject>();
        rainParticles = new List<GameObject>();

        for (int i = 0; i < maxRaindrops; i++)
        {
            GameObject dropObj = Instantiate(rainDropObject);
            dropObj.SetActive(false);
            dropObj.transform.parent = transform;
            rainDrops.Add(dropObj);

            GameObject particleObj = Instantiate(rainDropParticle);
            particleObj.SetActive(false);
            particleObj.transform.parent = transform;
            rainParticles.Add(particleObj);
        }

        dropObjectIndex = 0;
        particleIndex = 0;
    }

	private void Update()
    {
        // Pull out drop objects from pool if max number of spawned drops isn't reached
        if (spawnedRaindrops < maxRaindrops)
        {
            UpdateCameraBounds();

            // Find random x and y within (and slightly outside of) camera bounds
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            // Ensure object index is within pool size
            if (dropObjectIndex >= maxRaindrops) dropObjectIndex = 0;

            // Pull out a raindrop object, put it under rainsystem object and mark it active
            GameObject nextDrop = rainDrops[dropObjectIndex];
            nextDrop.transform.position = new Vector2(randomX, randomY);
            nextDrop.SetActive(true);

            spawnedRaindrops++;
            dropObjectIndex++;
        }
    }

    public void DespawnRaindrop(GameObject gameObj)
    {
        // Disable raindrop
        gameObj.SetActive(false);
        spawnedRaindrops--;

        // Ensure particle index is within pool size
        if (particleIndex >= maxRaindrops) particleIndex = 0;

        // Enable particle system at raindrops last position
        GameObject rainParticle = rainParticles[particleIndex];
        rainParticle.transform.position = gameObj.transform.position;
        rainParticle.SetActive(true);

        particleIndex++;
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
