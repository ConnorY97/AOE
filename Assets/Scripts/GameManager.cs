using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Public Vars
    public int mMaxTreeSpawn = 100;
    public GameObject mGround = null;
    public GameObject mTree = null;
    // Private Vars 
    private List<GameObject> mTrees = new List<GameObject>();



    // Start is called before the first frame update
    void Start()
    {
        if (mGround != null)
        {
            Mesh groundMesh = mGround.GetComponent<Mesh>();
            if (groundMesh != null)
            {
                Bounds bounds = groundMesh.bounds;

                float x = bounds.extents.x;
                float z = bounds.extents.z; // Get the half size in each direction

                for (int i = 0; i < mMaxTreeSpawn; i++)
                {
                    float posX = UnityEngine.Random.Range(-x, x);
                    float posZ = UnityEngine.Random.Range(-z, z);
                    GameObject tmp = Instantiate(mTree, new Vector3(posX, 0, posZ), transform.rotation);

                    mTrees.Add(tmp);
                }
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
