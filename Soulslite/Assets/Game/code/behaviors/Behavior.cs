using Pathfinding;
using UnityEngine;


public class Behavior
{
    private int currentWaypoint = 0;
    private bool wandering = false;

    public Path path;
    public float nextWaypointDistance = 2;


    public bool HasPath()
    {
        if (path != null && path.vectorPath != null)
        {
            return currentWaypoint < path.vectorPath.Count;
        }
        return false;
    }

    public void IncrementWaypoint()
    {
        currentWaypoint++;
    }

    public Vector2 GetNextWaypoint()
    {
        return path.vectorPath[currentWaypoint];
    }

    public bool WaypointReached(Vector2 currentPosition)
    {
        return Vector2.Distance(currentPosition, path.vectorPath[currentWaypoint]) < nextWaypointDistance;
    }

    public void SetPath(Seeker seeker, Vector2 startPosition, Vector2 targetPosition)
    {
        seeker.StartPath(startPosition, targetPosition, OnPathComplete);
        wandering = false;
    }

    public void Wander(Seeker seeker, Vector2 startPosition)
    {
        RandomPath rpath = RandomPath.Construct(startPosition, 32);
        rpath.spread = 32000;
        seeker.StartPath(rpath, OnPathComplete);
        wandering = true;
    }

    public void EndPath()
    {
        if (path != null)
        {
            path.Error();
            path = null;
        }
    }

    public bool IsWandering()
    {
        return wandering;
    }

    private void OnPathComplete(Path p)
    {
        p.Claim(this);
        if (!p.error)
        {
            if (path != null) path.Release(this);

            path = p;
            currentWaypoint = 0;
        }
        else
        {
            p.Release(this);
        }
    }
}
