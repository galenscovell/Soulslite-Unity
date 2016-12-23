using Pathfinding;
using UnityEngine;


public class SeekBehavior {
    private Vector2 target;
    private int currentWaypoint = 0;

    public Path path;
    public float nextWaypointDistance = 2;



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

    public void IncrementPath()
    {
        currentWaypoint++;
    }

    public Vector2 GetNextWaypoint()
    {
        return path.vectorPath[currentWaypoint];
    }

    public void SetPath(Seeker seeker, Vector2 startPosition, Rigidbody2D targetBody, float thinkTime)
    {
        // Start a new path to the targetPosition, return the result to the OnPathComplete function
        Vector2 trackedPosition = Vector2.zero;
        if (thinkTime > 0)
        {
            Vector2 currentTargetVelocity = targetBody.velocity;
            trackedPosition = targetBody.position + (targetBody.velocity * thinkTime);
        } else
        {
            trackedPosition = targetBody.position;
        }
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
