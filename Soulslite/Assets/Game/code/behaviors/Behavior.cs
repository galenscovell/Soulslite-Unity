using Pathfinding;
using UnityEngine;


public class Behavior
{
    private int currentWaypoint = 0;

    public Path path;
    public float nextWaypointDistance = 1;


    public bool HasPath()
    {
        if (path != null && path.vectorPath != null)
        {
            return currentWaypoint < path.vectorPath.Count;
        }
        return false;
    }

    public Vector2 GetNextWaypoint()
    {
        currentWaypoint++;

        if (HasPath())
        {
            return path.vectorPath[currentWaypoint];
        }
        else
        {
            return default(Vector2);
        }
    }

    public bool WaypointReached(Vector2 currentPosition)
    {
        if (HasPath())
        {
            return Vector2.Distance(currentPosition, path.vectorPath[currentWaypoint]) < nextWaypointDistance;
        }
        return true;
    }

    public void SetPath(Seeker seeker, Vector2 startPosition, Vector2 targetPosition)
    {
        seeker.StartPath(startPosition, targetPosition, OnPathComplete);
    }

    public void Wander(Seeker seeker, Vector2 startPosition)
    {
        RandomPath rpath = RandomPath.Construct(startPosition, 8);
        rpath.spread = 16000;
        seeker.StartPath(rpath, OnPathComplete);
    }

    public void EndPath()
    {
        if (path != null)
        {
            path.Error();
            path = null;
        }
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
