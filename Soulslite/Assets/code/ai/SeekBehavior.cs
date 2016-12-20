using Pathfinding;
using UnityEngine;


public class SeekBehavior {
    private Vector2 target;
    private int currentWaypoint = 0;

    public Path path;
    public float nextWaypointDistance = 2;



    public bool HasPath()
    {
        return path != null;
    }

    public Vector2 GetWaypoint()
    {
        return path.vectorPath[currentWaypoint];
    }

    public void SetPath(Seeker seeker, Vector2 startPosition, Vector2 targetPosition)
    {
        // Start a new path to the targetPosition, return the result to the OnPathComplete function
        target = targetPosition;
        seeker.StartPath(startPosition, targetPosition, OnPathComplete);
    }
	
    public Vector2 WalkPath(Rigidbody2D body, float speedMultiplier) {
        // We have no path to move after yet
        if (path == null) return default(Vector2);

        if (currentWaypoint > path.vectorPath.Count)
        {
            Debug.Log("End Of Path Reached");
            currentWaypoint++;
            path = null;
            return default(Vector2);
        }

        Vector2 nextWaypoint = path.vectorPath[currentWaypoint];
        if (Vector2.Distance(nextWaypoint, target) < 40)
        {
            Debug.Log("Within attack distance, ending path");
            return default(Vector2);
        }

        // Direction to the next waypoint
        Vector2 dir = ((Vector2) path.vectorPath[currentWaypoint] - body.position).normalized;

        // Check if we are close enough to the next waypoint
        // If we are, proceed to follow the next waypoint
        if (Vector3.Distance(body.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        return new Vector2(dir.x * speedMultiplier, dir.y * speedMultiplier);
    }

    private void OnPathComplete(Path p)
    {
        Debug.Log("A path was calculated -- did it have an error? " + p.error);

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
