using Pathfinding;
using UnityEngine;


public class SeekBehavior
{
    private Vector2 target;
    private int currentWaypoint = -1;

    public Path path;
    public float nextWaypointDistance = 2;


    public bool HasPath()
    {
        return path != null && currentWaypoint < path.vectorPath.Count;
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
        return Vector2.Distance(currentPosition, path.vectorPath[currentWaypoint]) < nextWaypointDistance;
    }

    public void SetPath(Seeker seeker, Vector2 startPosition, Vector2 trackedPosition)
    {
        target = trackedPosition;
        seeker.StartPath(startPosition, target, OnPathComplete);
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
