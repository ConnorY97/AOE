using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Public Vars
    public int mMaxTreeSpawn = 100;
    public int mMaxHumanSpawn = 1;
    public GameObject mGround = null;
    public GameObject mTree = null;
    public GameObject mHuman = null;
    // Private Vars 
    private List<GameObject> mTrees = new List<GameObject>();
    private List<GameObject> mHumans = new List<GameObject>();
    private Human mCurrentHuman = null;
    private Tree mCurrentTree = null;

    // Singleton Functions
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (mGround != null)
        {
            MeshCollider groundMesh = mGround.GetComponent<MeshCollider>();
            if (groundMesh != null)
            {
                // Tree spawning
                Bounds bounds = groundMesh.bounds;

                float x = bounds.extents.x;
                float z = bounds.extents.z; // Get the half size in each direction

                for (int i = 0; i < mMaxTreeSpawn; i++)
                {
                    float posX = UnityEngine.Random.Range(-x, x);
                    float posZ = UnityEngine.Random.Range(-z, z);
                    GameObject tmp = Instantiate(mTree, new Vector3(posX, 1, posZ), transform.rotation);

                    tmp.name = $"Tree{i}";
                    mTrees.Add(tmp);
                }
                // --

                // Human spawning
                for (int i = 0; i < mMaxHumanSpawn; i++)
                {
                    GameObject tmp = Instantiate(mHuman, new Vector3(0, 1, 0), transform.rotation);

                    tmp.name = $"Human{i}";
                    mHumans.Add(tmp);
                }
                //--
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Public Functions
    public void SetClickedObject(GameObject selectedObject)
    {
        if (selectedObject != null)
        {
            if (selectedObject.GetComponent<Human>() != null)
            {
                mCurrentHuman = selectedObject.GetComponent<Human>();
            }
            else if (selectedObject.GetComponent<Tree>() != null)
            {
                mCurrentTree = selectedObject.GetComponent<Tree>();

                if (mCurrentHuman != null)
                {
                    mCurrentHuman.SetTarget(mCurrentTree.gameObject);
                }
            }
            else
            {
                mCurrentHuman = null;
                mCurrentTree = null;
            }
        }
    }

    
}
