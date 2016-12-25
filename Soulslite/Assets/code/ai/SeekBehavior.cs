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

    public void IncrementPath()
    {
        currentWaypoint++;
    }

    public Vector2 GetNextWaypoint()
    {
        return path.vectorPath[currentWaypoint];
    }

    public bool HasReachedWaypoint(Vector2 currentPosition)
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
            // Reset the waypoint counter
            currentWaypoint = 0;
        }
        else
        {
            p.Release(this);
        }
    }
}
