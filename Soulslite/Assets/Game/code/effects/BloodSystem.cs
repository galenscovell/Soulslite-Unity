using System.Collections.Generic;
using UnityEngine;


public class BloodSystem : MonoBehaviour
{
    public static BloodSystem bloodSystem;

    public GameObject bloodSplashObject;

    private List<GameObject> bloodParticleObjects;
    private int spawnedBloodObjects;
    private int bloodObjectIndex;
    private int maxBloodObjects = 30;


    private void Awake()
    {
        // Singleton, destroyed between scenes
        if (bloodSystem != null) Destroy(bloodSystem);
        else bloodSystem = this;
    }

    private void Start () {
        // Create an object pool for blood particle objects
        bloodParticleObjects = new List<GameObject>();

        for (int i = 0; i < maxBloodObjects; i++)
        {
            GameObject bloodObj = Instantiate(bloodSplashObject);
            bloodObj.SetActive(false);
            bloodObj.transform.parent = transform;
            bloodParticleObjects.Add(bloodObj);
        }

        bloodObjectIndex = 0;
    }

    public void SpawnBlood(Vector2 position, Vector2 direction)
    {
        // Spawn blood effect at position moving in a direction
        if (spawnedBloodObjects < maxBloodObjects)
        {
            // Ensure object index is within pool size
            if (bloodObjectIndex >= maxBloodObjects) bloodObjectIndex = 0;

            GameObject nextObj = bloodParticleObjects[bloodObjectIndex];

            // Rotate blood to spray in collision direction, set it to target position, and mark it active
            nextObj.transform.rotation = Quaternion.LookRotation(direction);
            nextObj.transform.position = position;
            nextObj.SetActive(true);

            spawnedBloodObjects++;
            bloodObjectIndex++;
        }

    }

    public void DespawnBlood(GameObject gameObj)
    {
        // Disable object
        gameObj.SetActive(false);
        spawnedBloodObjects--;
        bloodObjectIndex++;
    }
}
