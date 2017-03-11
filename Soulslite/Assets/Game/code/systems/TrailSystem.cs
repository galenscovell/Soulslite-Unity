using System.Collections.Generic;
using UnityEngine;


public class TrailSystem : MonoBehaviour
{
    public static TrailSystem trailSystem;

    private List<DashTrailObject> trailObjectsInUse;
    private List<DashTrailObject> trailObjects;

    private float spawnTimer;
    private bool on;

    private int trailIndex;
    private int maxTrails = 30;

    public SpriteRenderer leadingSprite;
    public DashTrailObject trailObject;
    public Color[] colorPool;

    private int colorIndex = 0;
    private int segments = 5;
    private int currentSegment;
    private List<float> segmentLifetimes = new List<float> { 1f, 0.8f, 0.7f, 0.5f, 0.4f };
    private List<float> spawnTimes = new List<float> { 0, 0.05f, 0.07f, 0.09f, 0.11f };


    private void Awake()
    {
        if (trailSystem != null) Destroy(trailSystem);
        else trailSystem = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
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

            if (currentSegment < segments)
            {
                if (spawnTimer > spawnTimes[currentSegment])
                { 
                    if (trailIndex >= maxTrails) trailIndex = 0;

                    DashTrailObject trailObject = trailObjects[trailIndex];
                    trailObject.Initiate(segmentLifetimes[currentSegment], leadingSprite.sprite, transform.position, colorPool[colorIndex]);
                    trailObjectsInUse.Add(trailObject);
                    trailObject.SetActive(true);

                    trailIndex++;
                    currentSegment++;
                    colorIndex++;

                    if (colorIndex >= colorPool.Length)
                    {
                        colorIndex = 0;
                    }
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
        spawnTimer = 0;
        currentSegment = 0;
    }

    public void EndTrail()
    {
        on = false;
    }
}