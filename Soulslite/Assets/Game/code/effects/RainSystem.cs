using UnityEngine;


public class RainSystem : MonoBehaviour
{
    private int spawnedRaindrops;
    private float minX, maxX, minY, maxY;

    public new Camera camera;
    public GameObject rainDrop;
    public int maxRaindrops;
    public int raindropSpeed;


	private void FixedUpdate()
    {
		if (Input.GetButtonDown("Button1"))
        {
            SpawnRaindrop();
            SpawnRaindrop();
        }
	}

    private void SpawnRaindrop()
    {
        if (spawnedRaindrops < maxRaindrops)
        {
            // Pick random spot within distance of camera (including inside of it)
            UpdateCameraBounds();

            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            GameObject newDrop = Instantiate(rainDrop, new Vector2(randomX, randomY), Quaternion.identity);
            newDrop.SetActive(true);

            spawnedRaindrops++;
        }
    }

    private void UpdateCameraBounds()
    {
        Vector2 cameraBottomLeft = camera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 cameraTopRight = camera.ViewportToWorldPoint(new Vector2(1, 1));

        minX = cameraBottomLeft.x;
        maxX = cameraTopRight.x;

        minY = cameraBottomLeft.y;
        maxY = cameraTopRight.y;
    }
}
