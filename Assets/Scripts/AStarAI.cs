using UnityEngine;
// Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
// This line should always be present at the top of scripts which use pathfinding
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class AStarAI : MonoBehaviour
{
    private Transform targetPositionTransform;
    private Vector2 targetPositionVector3;
    private bool useTransform;

    private Vector3 start, end;

    private float startPathOffset;

    private Seeker seeker;

    public Path path;

    private float speed = 2;

    public float nextWaypointDistance = 3;

    private int currentWaypoint = 0;

    public float repathRate = 0.5f;
    private float lastRepath = float.NegativeInfinity;

    public bool reachedEndOfPath;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        // Path pooling. To avoid unnecessary allocations paths are reference counted.
        // Calling Claim will increase the reference count by 1 and Release will reduce
        // it by one, when it reaches zero the path will be pooled and then it may be used
        // by other scripts. The ABPath.Construct and Seeker.StartPath methods will
        // take a path from the pool if possible. See also the documentation page about path pooling.
        p.Claim(this);
        if (!p.error)
        {
            if (path != null) path.Release(this);
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }
        else
        {
            p.Release(this);
        }
    }

    public void Update()
    {
        start = transform.position + new Vector3(0, startPathOffset, 0);

        if (Time.time > lastRepath + repathRate && seeker.IsDone())
        {
            lastRepath = Time.time;          

            // Start a new path to the targetPosition, call the the OnPathComplete function
            // when the path has been calculated (which may take a few frames depending on the complexity)
            if (useTransform)
                seeker.StartPath(start, targetPositionTransform.position, OnPathComplete);
            else
                seeker.StartPath(start, targetPositionVector3, OnPathComplete);
        }

        if (path == null)
        {
            // We have no path to follow yet, so don't do anything
            return;
        }

        // Check in a loop if we are close enough to the current waypoint to switch to the next one.
        // We do this in a loop because many waypoints might be close to each other and we may reach
        // several of them in the same frame.
        reachedEndOfPath = false;
        // The distance to the next waypoint in the path
        float distanceToWaypoint;
        while (true)
        {
            // If you want maximum performance you can check the squared distance instead to get rid of a
            // square root calculation. But that is outside the scope of this tutorial.

            //distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            distanceToWaypoint = Vector3.Distance(start, path.vectorPath[currentWaypoint]);

            if (distanceToWaypoint < nextWaypointDistance)
            {
                // Check if there is another waypoint or if we have reached the end of the path
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    // Set a status variable to indicate that the agent has reached the end of the path.
                    // You can use this to trigger some special code if your game requires that.
                    reachedEndOfPath = true;
                    break;
                }
            }
            else
            {
                break;
            }
        }

        // Slow down smoothly upon approaching the end of the path
        // This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

        // Direction to the next waypoint
        // Normalize it so that it has a length of 1 world unit

        //Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 dir = (path.vectorPath[currentWaypoint] - start).normalized;

        // Multiply the direction by our desired speed to get a velocity
        Vector3 velocity = dir * speed * speedFactor;


        transform.position += velocity * Time.deltaTime;
    }

    //permette di seguire una posizione fissa (per i nemici che si muovono verso direzioni casuali)
    public void SetTarget(bool isTransform, Vector3 target)
    {
        targetPositionVector3 = target;
        useTransform = isTransform;
    }

    //permette di seguire un bersaglio mobile (di solito il player)
    public void SetTarget(bool isTransform, Transform target)
    {
        targetPositionTransform = target;
        useTransform = isTransform;
    }

    public void SetSpeed(float enemySpeed)
    {
        speed = enemySpeed;
    }

    public void SetStartPathOffset(float offset)
    {
        startPathOffset = offset;
    }


}