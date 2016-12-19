using Pathfinding;
using UnityEngine;


public class SeekBehavior : MonoBehaviour {
    public Transform target;                   // point to move to
    public Path path;                          // calculated path
    public float speed = 2;                    // speed per second
    public float nextWaypointDistance = 3;     // max distance from ai to a waypoint for it to continue to the next

    private int currentWaypoint = 0;           // current waypoint moving towards
    private Seeker seeker;                     // the entity


	private void Start () {
        seeker = GetComponent<Seeker>();

        // Start a new path to the targetPosition, return the result to the OnPathComplete function
        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }
	
	private void FixedUpdate () {
        if (path == null)
        {
            // We have no path to move after yet
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("End Of Path Reached");
            return;
        }

        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;
        this.gameObject.transform.Translate(dir);

        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
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
