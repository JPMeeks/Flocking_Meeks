using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BehaviourManager : MonoBehaviour
{
    public int humCount;        // amount of humans
    public List<GameObject> hums;   // list of humans

    public int treeCount;        // amount of obstacles
    public List<GameObject> trees;   // list of collision obstacles

    public GameObject humPrototype; // human to instantiate
    public GameObject treePrototype; // human to instantiate
    public GameObject locate;

    public Terrain terrain;         // terrain that is being walked on
    public Vector3 worldSize;       // world size
    private TerrainManager terrainMngr;

    private List<BoundingSphere> humBS;
    private List<MovementForces> humMF;

    public List<BoundingSphere> treeBS;

    public Vector3 avgPos;
    public Vector3 avgDir;

    public float timeFromStart = 0;

    // Materials
    public Material matRed;
    public Material matGreen;
    public Material matBlue;
    public Material matBlack;
    public Material matWhite;

    // Use this for initialization
    void Start()
    {
        
        // instantiate lists
        hums = new List<GameObject>();
        humBS = new List<BoundingSphere>();
        humMF = new List<MovementForces>();

        // check human is assigned
        if (null == humPrototype)
        {
            Debug.Log("ERROR: HUMAN GAMEOBJECT IS NOT DEFINED");
            Debug.Break();
        }
        // check if terrain is assigned
        if (null == terrain)
        {
            Debug.Log("ERROR: TERRAIN IS NOT DEFINED");
            Debug.Break();
        }
        terrainMngr = terrain.GetComponent<TerrainManager>();

        // instantiate human
        for (int x = 0; x < humCount; x++)
        {
            GameObject temp = Instantiate(humPrototype);
            hums.Add(temp);
        }
        for (int x = 0; x < treeCount; x++)
        {
            GameObject temp = Instantiate(treePrototype);
            trees.Add(temp);
        }

        // initialize the world with terrainManager worldSize
        worldSize = terrainMngr.worldSize;

        // initalize boundingSpheres
        for (int x = 0; x < hums.Count; x++)
        {
            humBS.Add(hums[x].GetComponent<BoundingSphere>());
        }
        for (int x = 0; x < trees.Count; x++)
        {
            treeBS.Add(trees[x].GetComponent<BoundingSphere>());
        }

        // initalize movementForces
        for (int x = 0; x < hums.Count; x++)
        {
            humMF.Add(hums[x].GetComponent<MovementForces>());
        }

        // randomize positions
        for (int x = 0; x < hums.Count; x++)
        {
            RandomizePosition(hums[x]);
        }
        for (int x = 0; x < trees.Count; x++)
        {
            RandomizePosition(trees[x]);
        }

        avgPos = new Vector3();
        avgDir = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        // find and apply average direction and center
        avgPos = FindCentroid();
        avgDir = FindDirection();
        locate.transform.position = avgPos;

        // human check if they are too close to a human
        for (int x = 0; x < hums.Count; x++)
        {
            for (int y = 0; y < hums.Count; y++)
            {
                if(x == y)
                {
                    // do nothing
                }
                else
                {
                    if(humBS[x].IsThreatened(humBS[y]))
                    {
                        humMF[x].ApplyForce(humMF[x].Flee(humMF[y].pos) * 6);
                    }
                }
            }
        }

        // human check if they are too far to any human
        for (int x = 0; x < hums.Count; x++)
        {
            if (humBS[x].IsLonely(humBS))
            {
                Vector3 closest = FindClosestMember(x);
                humMF[x].ApplyForce(humMF[x].Seek(closest));
            }
        }

        timeFromStart += Time.deltaTime;
    }

    //calculates the position of the object at random
    void RandomizePosition(GameObject theObject)
    {
        // Set position of target based on the size of the world
        Vector3 position = new Vector3(Random.Range(0.0f, worldSize.x), 0.0f, Random.Range(0.0f, worldSize.z));
        // set the height of the object based on the position of the terrain
        position.y = terrainMngr.GetHeight(position) + 1.0f;
        // set the position of target back
        theObject.transform.position = position;
    }

    void TurnHuman()
    {
        for(int z = 0; z < hums.Count; z++)
        {
            humMF[z].move = false;
            humMF[z].wander = true;
        }
    }

    Vector3 FindClosestMember(int loopPos)
    {
        Vector3 closest = new Vector3();
        int checkTarget = 0;

        for (int x = 0; x < humBS.Count; x++)
        {
            if(x != loopPos)
            {
                int temp1 = (int)humBS[x].CheckDist(humBS[x]);
                int temp2 = (int)humBS[checkTarget].CheckDist(humBS[x]);
                if (temp1 <= temp2)
                    checkTarget = x;
            }
        }

        closest = humMF[checkTarget].pos;
        return closest;
    }

    public Vector3 FindCentroid()
    {
        Vector3 average = new Vector3();

        for(int x = 0; x < hums.Count; x++)
        {
            average += humMF[x].pos;
        }
        average.x = average.x / hums.Count;
        average.z = average.z / hums.Count;
        average.y = terrain.SampleHeight(average);

        return average;
    }
    public Vector3 FindDirection()
    {
        Vector3 average = new Vector3();

        for (int x = 0; x < hums.Count; x++)
        {
            average += humMF[x].dir;
        }
        average.x = average.x / hums.Count;
        average.z = average.z / hums.Count;
        average.y = 0;

        return average;
    }
}
