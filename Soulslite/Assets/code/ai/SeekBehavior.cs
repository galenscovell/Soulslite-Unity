using Pathfinding;
using UnityEngine;


public class SeekBehavior {
    public Vector2 target;                     // point to move to
    public Path path;                          // calculated path
    public float nextWaypointDistance = 2;     // max distance from ai to a waypoint for it to continue to the next

    private int currentWaypoint = 0;           // current destination index



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
        seeker.StartPath(startPosition, targetPosition, OnPathComplete);
    }
	
    public void WalkPath(Rigidbody2D body, Vector2 nextVelocity, float speedMultiplier) {
        // We have no path to move after yet
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("End Of Path Reached");
            path = null;
            return;
        }

        Vector2 nextWaypoint = path.vectorPath[currentWaypoint];
        if (Vector2.Distance(nextWaypoint, target) < 40)
        {
            Debug.Log("Within attack distance, ending path");
            return;
        }

        // Direction to the next waypoint
        Vector3 dir = ((Vector2) path.vectorPath[currentWaypoint] - target).normalized;
        nextVelocity.x = dir.x * speedMultiplier;
        nextVelocity.y = dir.y * speedMultiplier;

        // Check if we are close enough to the next waypoint
        // If we are, proceed to follow the next waypoint
        if (Vector3.Distance(body.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    private void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            // Reset the waypoint counter
            currentWaypoint = 0;
        }
    }
}
