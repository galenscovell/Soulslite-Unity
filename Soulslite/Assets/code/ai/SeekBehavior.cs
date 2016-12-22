using Pathfinding;
using UnityEngine;


public class SeekBehavior {
    private Vector2 target;
    private int currentWaypoint = 0;
    private int aggroDistance = 128;

    public Path path;
    public float nextWaypointDistance = 2;


    public bool InAggroRange(Vector2 currentPosition, Vector2 targetPosition)
    {
        return Vector2.Distance(currentPosition, targetPosition) < aggroDistance;
    }

    public void RemovePath()
    {
        if (path != null)
        {
            path.Release(this);
            path = null;
        }
    }

    public bool HasPath()
    {
        return path != null && currentWaypoint < path.vectorPath.Count;
    }

    public bool HasReachedWaypoint(Vector2 currentPosition)
    {
        return Vector2.Distance(currentPosition, path.vectorPath[currentWaypoint]) < nextWaypointDistance;
    }

    public bool InAttackRange(Vector2 currentPosition)
    {
        return Vector2.Distance(currentPosition, target) < 48;
    }

    public void IncrementPath()
    {
        currentWaypoint++;
    }

    public Vector2 GetNextWaypoint()
    {
        return path.vectorPath[currentWaypoint];
    }

    public void SetPath(Seeker seeker, Vector2 startPosition, Vector2 targetPosition)
    {
        // Start a new path to the targetPosition, return the result to the OnPathComplete function
        target = targetPosition;
        seeker.StartPath(startPosition, targetPosition, OnPathComplete);
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
