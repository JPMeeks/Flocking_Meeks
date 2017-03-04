using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// JP Meeks
// jpm4447@g.rit.edu
// this script is designed to work in conjunction with the gameManager in order to move characters around a field

public class MovementForces : MonoBehaviour
{
    public Vector3 pos;     // position of object
    public Vector3 dir;     // the direction faced
    public Vector3 vel;     // current velocity of object
    public Vector3 acc;     // sum of forces acting on the object
    public Vector3 fullSight;   // the max distance an object can detect obstacles
    public Vector3 halfSight;   // half the max distance an object detects obstacles

    public float mass = 2.0f;
    public float maxVel = 50.0f;    // maximum speed that can be reached
    public float time = 2.0f;       // time the evasion and pursuit methods track
    public float MAX_SIGHT;         // the maximum range an object can detect an obstacle to avoid
    public float MAX_AVOID;         // the maximum amount allowed for an avoidance vector

    private BehaviourManager behaMngr;
    private Vector3 worldSize;
    private GameObject target;
    private Vector3 futPos;         // the future position of the target

    public bool move = true;
    public bool wander = true;
    public bool debug = true;
    public bool smart = true;

    private List<BoundingSphere> treeBS; 
    private float timeFromCreate;
    private Vector3 origin;

	// Use this for initialization
	void Start ()
    {
        treeBS = new List<BoundingSphere>();
        
        GameObject gameMngr = GameObject.Find("GameManager");
        if(null == gameMngr)
        {
            Debug.Log("ERROR: NO GAME MANAGER FOUND");
            Debug.Break();
        }

        pos = transform.position;
        behaMngr = gameMngr.GetComponent<BehaviourManager>();
        worldSize = behaMngr.worldSize;
        origin = new Vector3(worldSize.x / 2, 1, worldSize.z / 2);

        treeBS = behaMngr.treeBS;
        //Debug.Log(treeBS.Count);

        // check mass to make sure it is positive
        if (mass <= 0.0f)
        {
            mass = 0.5f;
        }
	}

    // set the thing target of the object
    public void SetTarget(GameObject targe)
    {
        target = targe;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // check whether to toggle debug lines
        if (Input.GetKeyDown("d"))
            debug = !debug;

        if (move == true)
            wander = false;
        else if (move == false)
            wander = true;
        
        if(timeFromCreate > 2.0f)
        {
            UpdatePosition();
            SetTransform();
            TurnAround();
        }

        timeFromCreate += Time.deltaTime;
    }

    // Update the position based on the velocity and acceleration
    void UpdatePosition()
    {
        //Step 0: update position to current tranform
        pos = transform.position;
        pos.y = behaMngr.terrain.SampleHeight(pos)+2;

        //Step 1/2: find the target
        Vector3 seekingForce = Seek(FindWanderPoint(behaMngr.avgPos, Vector3.ClampMagnitude(behaMngr.avgDir*600, maxVel)))*4;
        
        ApplyForce(seekingForce);

        // apply object avoidance
        for (int x = 0; x < treeBS.Count; x++)
        {
            if (IntersectsCircle(fullSight, halfSight, treeBS[x].transform.position, treeBS[x].radius))
            {
                ApplyForce(Avoid(treeBS[x]));
                break;
            }  
        }

        // Step 1: Add Acceleration to Velocity * Time
        vel += acc * Time.deltaTime;

        // Step 2: Add vel to position * Time
        pos += vel * Time.deltaTime;

        // Step 3: Reset Acceleration vector
        acc = Vector3.zero;

        // Step 4: Calculate direction (to know where we are facing)
        dir = vel.normalized;
    }


    //Apply a force to the vehicle
    public void ApplyForce(Vector3 force)
    {
        //F = M * A
        //F / M = M * A / M
        //F / M = A * (M / M)
        //F / M = A * 1
        //A = F / M
        acc += force / mass;
    }

    public Vector3 FindFuturePosition(Vector3 targetPos, Vector3 targetVel)
    {
        Vector3 futurePositon = new Vector3();

        futurePositon = targetPos + targetVel * time;

        return futurePositon;
    }

    public Vector3 Seek(Vector3 targetPosition)
    {
        //Step 1: Calculate the desired unclamped velocity
        //which is from this vehicle to target's position
        Vector3 desiredVelocity = targetPosition - pos;

        // Step 2: Calculate maximum speed so the vehicle does not move faster than it should
        // desiredVelocity.Normalize();
        // desiredVelocity *= maxVel;

        //Step 2 Alternative:
        desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxVel);

        //Step 3: Calculate steering force
        Vector3 steeringForce = desiredVelocity - vel;

        //Step 4: return the force so it can be applied to this vehicle
        return steeringForce;
    }

    public Vector3 Flee(Vector3 targetPosition)
    {
        //Step 1: Calculate the desired unclamped velocity which is from this vehicle to target's position
        Vector3 desiredVelocity = pos - targetPosition;

        // Step 2: Calculate maximum speed so the vehicle does not move faster than it should
        //desiredVelocity.Normalize();
        //desiredVelocity *= maxVel;

        //Step 2 Alternative:
        desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxVel);

        //Step 3: Calculate steering force
        Vector3 steeringForce = desiredVelocity - vel;

        //Step 4: return the force so it can be applied to this vehicle
        return steeringForce;
    }

    //Apply friction to the vehicle based on the coefficient
    void ApplyFriction(float coeff)
    {
        // Step 1: Oposite velocity
        Vector3 friction = vel * -1.0f;
        // Step 2: Normalize so is independent of velocity
        friction.Normalize();
        // Step 3: Multiply by coefficient
        friction = friction * coeff;
        // Step 4: Add friction to acceleration
        acc += friction;
    }

    //Apply the trasformation
    void SetTransform()
    {
        transform.position = pos;
        //orient the object
        transform.right = dir;
    }

    void TurnAround()
    {
        if(pos.x >= (worldSize.x - (worldSize.x * .15)))
        {
            Vector3 toOrigin = 2*Seek(origin);
            ApplyForce(toOrigin);
        }
        else if (pos.x <= (worldSize.x * .15))
        {
            Vector3 toOrigin = 2*Seek(origin);
            ApplyForce(toOrigin);
        }

        if (pos.z >= (worldSize.z - (worldSize.z * .15)))
        {
            Vector3 toOrigin = 2*Seek(origin);
            ApplyForce(toOrigin);
        }
        else if (pos.z <= (worldSize.z * .15))
        {
            Vector3 toOrigin = 2*Seek(origin);
            ApplyForce(toOrigin);
        }
    }

    // calculate the wander target point
    Vector3 FindWanderPoint(Vector3 point, Vector3 velocity)
    {
        Vector3 wanderCenter = FindFuturePosition(point, velocity);

        float x = wanderCenter.x + 4 * Mathf.Cos(Random.Range(0.0f, 180.0f) * Mathf.PI / 180);
        float z = wanderCenter.z + 4 * Mathf.Sin(Random.Range(0.0f, 180.0f) * Mathf.PI / 180); ;

        Vector3 wanderPoint = new Vector3(x, 1, z);
        wanderPoint.y = behaMngr.terrain.SampleHeight(wanderPoint);

        return wanderPoint;
    }

    // calculates the current sight being determined
    void SetSight()
    {
        fullSight = pos + vel.normalized * MAX_SIGHT;
        halfSight = pos + vel.normalized * MAX_SIGHT * (float).5;
    }

    float ObstacleDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));
    }

    bool IntersectsCircle(Vector3 ahead, Vector3 ahead2, Vector3 obsCenter, float radius)
    {
        return (ObstacleDistance(obsCenter, FindFuturePosition(pos,vel)) <= radius || ObstacleDistance(obsCenter, FindFuturePosition(pos, vel)/2) <= radius);
    }

    // calculate avoidance force
    Vector3 Avoid(BoundingSphere bs)
    {
        Vector3 avoidForce = new Vector3();
        
        //Step 1: Calculate the desired unclamped velocity which is from this vehicle to target's position
        avoidForce = fullSight - bs.transform.position;

        //Step 2 Alternative:
        //avoidForce = Vector3.ClampMagnitude(avoidForce, MAX_AVOID*60);

        //Step 3: Calculate steering force
        Vector3 steeringForce = avoidForce - vel;

        //Step 4: return the force so it can be applied to this vehicle
        return steeringForce*10;
    }

    void OnRenderObject()
    {
        if(debug)
        {
            GL.PushMatrix();
                        
            // find group average position
            GL.Begin(GL.LINES);
            behaMngr.matBlack.SetPass(0);
            GL.Color(Color.black);
            GL.Vertex(behaMngr.avgPos);
            GL.Vertex(behaMngr.avgPos + (Quaternion.Euler(0, 90, 0) * vel).normalized * 1.0f);
            GL.Vertex(behaMngr.avgPos);
            GL.Vertex(behaMngr.avgPos + (Quaternion.Euler(0, -90, 0) * vel).normalized * 1.0f);
            GL.Vertex(behaMngr.avgPos);
            GL.Vertex(behaMngr.avgPos + (Quaternion.Euler(0, 180, 0) * vel).normalized * 1.0f);
            GL.Vertex(behaMngr.avgPos);
            GL.Vertex(behaMngr.avgPos + (Quaternion.Euler(0, -180, 0) * vel).normalized * 1.0f);
            GL.End();

            GL.Begin(GL.LINES);
            behaMngr.matBlack.SetPass(0);
            GL.Color(Color.black);
            GL.Vertex(behaMngr.avgPos);
            GL.Vertex(behaMngr.avgPos + behaMngr.avgDir.normalized * 16.0f);
            GL.End();

            GL.PopMatrix();
        }
    }
}
