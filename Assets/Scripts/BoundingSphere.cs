using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// JP Meeks
// jpm4447@g.rit.edu


public class BoundingSphere : MonoBehaviour
{
    private Vector3 position;
    public float radius = 1.0f;
    public bool colliding = false;
    
    public float threatRad = 1.0f;
    public float friendRad = 5.0f;

    // Use this for initialization
    void Start()
    {
        if (radius <= 0.0f)
        {
            radius = 1.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        position = gameObject.transform.position;
    }

    public bool IsColliding(BoundingSphere other)
    {
        bool output = false;
        if (radius + other.radius > Vector3.Distance(position, other.position))
        //if(((radius + other.radius) * (radius + other.radius)) >
        //   ((other.position.x - position.x) * (other.position.x - position.x) + 
        //    (other.position.z - position.z) * (other.position.z - position.z)))
        {
            output = true;
            //Debug.Log("Hit!");
        }
        return output;
    }

    public bool IsThreatened(BoundingSphere other)
    {
        bool output = false;
        if (threatRad + other.threatRad > Vector3.Distance(position, other.position))
        {
            output = true;
            //Debug.Log("Hit!");
        }
        return output;
    }

    public bool IsLonely(List<BoundingSphere> BS)
    {
        bool output = false;
        for (int x = 0; x < BS.Count; x++)
        {
            if (friendRad + BS[x].threatRad > Vector3.Distance(position, BS[x].position))
            {
                output = true;
                //Debug.Log("Hit!");
            }
        }
        
        return output;
    }

    public float CheckDist(BoundingSphere other)
    {
        float output = Vector3.Distance(position, other.position);

        return output;
    }
}
