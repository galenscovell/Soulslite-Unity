using System.Collections.Generic;
using UnityEngine;


public class TrailSystem : MonoBehaviour
{
    public static TrailSystem trailSystem;

    private List<DashTrailObject> trailObjectsInUse;
    private List<DashTrailObject> trailObjects;

    private float spawnInterval;
    private float spawnTimer;
    private bool on;

    private int trailIndex;
    private int maxTrails = 40;

    public SpriteRenderer leadingSprite;
    public DashTrailObject trailObject;
    public int segments;
    public float segmentLifetime;


    private void Awake()
    {
        if (trailSystem != null) Destroy(trailSystem);
        else trailSystem = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        spawnInterval = segmentLifetime / segments / 2;

        trailObjectsInUse = new List<DashTrailObject>();
        trailObjects = new List<DashTrailObject>();

        for (int i = 0; i < maxTrails; i++)
        {
            DashTrailObject trail = Instantiate(trailObject);
            trail.SetActive(false);
            trail.transform.SetParent(transform);
            trailObjects.Add(trail);
        }

        trailIndex = 0;
        on = false;
    }

    private void Update()
    {
        if (on)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer > spawnInterval)
            {
                if (trailObjectsInUse.Count < segments)
                {
                    if (trailIndex >= maxTrails) trailIndex = 0;

                    DashTrailObject trailObject = trailObjects[trailIndex];
                    trailObject.Initiate(segmentLifetime, leadingSprite.sprite, transform.position);
                    trailObjectsInUse.Add(trailObject);
                    trailObject.SetActive(true);
                    spawnTimer = 0;

                    trailIndex++;
                }
            }
        }
    }

    public void RemoveTrailObject(DashTrailObject obj)
    {
        obj.SetActive(false);
        trailObjectsInUse.Remove(obj);
    }

    public void BeginTrail()
    {
        on = true;
        spawnTimer = spawnInterval;
    }

    public void EndTrail()
    {
        on = false;
    }
}