using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum ResourceType
{
    WOOD,
    ORE,
    COAL,
    MEAT,
    MAX
}

public class GameManager : MonoBehaviour
{
    // Public Vars
    // Spawn Values
    public int mMaxHumanSpawn = 1;
    public int mMaxTreeSpawn = 100;
    public int mMaxOreSpawn = 50;
    public float mSpawnDistance = 3;
    // Prefabs
    public GameObject mGround = null;
    public GameObject mTree = null;
    public GameObject mHuman = null;
    public GameObject mOre = null;
    // UI
    public Image mCurrentSelectedIcon = null;
    public TMP_Text mSelectedHitPoints = null;
    public List<TMP_Text> mResourceCountUI = new List<TMP_Text>();
    public Button mSpeedIncrease = null;
    // Game objects
    public GameObject mHome = null;
    // Private Vars
    private List<GameObject> mTrees = new List<GameObject>();
    private List<GameObject> mHumans = new List<GameObject>();
    private List<GameObject> mOres = new List<GameObject>();
    private Human mCurrentHuman = null;
    private NavMeshSurface mGroundSurface = null;
    private Dictionary<ResourceType, float> mTotalResources = new Dictionary<ResourceType, float>();
    private List<Vector3> mResourcePositions = new List<Vector3>();

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
        // Add the town to the invalid positions
        mResourcePositions.Add(mHome.transform.position);
        if (mGround != null)
        {
            MeshCollider groundMesh = mGround.GetComponent<MeshCollider>();
            if (groundMesh != null)
            {
                // Getting the spawning bounds
                Bounds bounds = groundMesh.bounds;

                float x = bounds.extents.x;
                float z = bounds.extents.z; // Get the half size in each direction

                float halfX = x / 5;
                float halfZ = z / 5;
                //---
                // Create the nav mesh surface before the human,
                //  Otherwise it gets mad there is not surface
                mGroundSurface = mGround.GetComponent<NavMeshSurface>();

                if (mGroundSurface != null)
                {
                    mGroundSurface.BuildNavMesh();
                }
                else
                {
                    Debug.Log("Failed to create navmesh surface");
                }
                //--
                // Human spawning
                for (int i = 0; i < mMaxHumanSpawn; i++)
                {
                    float posX = UnityEngine.Random.Range(-halfX, halfX);
                    float posZ = UnityEngine.Random.Range(-halfZ, halfZ);
                    Vector3 newPos = new Vector3(posX, 1.25f, posZ);
                    GameObject tmp = Instantiate(mHuman, newPos, transform.rotation);


                    // Add the humans to the invalid positions
                    mResourcePositions.Add(newPos);

                    tmp.name = $"Human{i}";

                    Color uniqueColor = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
                    // Set the unique icon colour for each human
                    Human human = tmp.GetComponent<Human>();
                    if (human != null)
                    {
                        human.IconColor = uniqueColor;
                    }

                    // Zero out the vel to try and stop the weird movement after spawning
                    if (human.Rigidbody != null)
                    {
                        human.Rigidbody.velocity = Vector3.zero;
                    }

                    // Set the material to match the icon color
                    Material material = tmp.GetComponent<Renderer>().material;
                    if (material != null)
                    {
                        material.color = uniqueColor;
                    }

                    mHumans.Add(tmp);
                }
                //--
                // Tree spawning
                for (int i = 0; i < mMaxTreeSpawn; i++)
                {
                    GameObject tmp = Instantiate(mTree, GetFreePosition(bounds), transform.rotation);

                    tmp.name = $"Tree{i}";

                    tmp.GetComponent<Trees>().Init(ResourceType.WOOD, 100.0f, 10.0f);

                    mTrees.Add(tmp);
                }
                // --
                // Ore spawning
                for (int i = 0; i < mMaxOreSpawn; i++)
                {
                    GameObject tmp = Instantiate(mOre, GetFreePosition(bounds), transform.rotation);

                    tmp.name = $"Ore{i}";

                    tmp.GetComponent<Ore>().Init(ResourceType.ORE, 100.0f, 10.0f);

                    mOres.Add(tmp);
                }

                // Regenerate the navmesh now all resources have spawned
                if (mGroundSurface != null)
                {
                    mGroundSurface.BuildNavMesh();
                }
                else
                {
                    Debug.Log("Failed to create navmesh surface");
                }
            }

            if (mCurrentSelectedIcon != null)
            {
                mCurrentSelectedIcon.gameObject.SetActive(false);
            }
        }

        // Set buttons as uninteractable
        mSpeedIncrease.interactable = false;

        // Set up resource dictionary
        for (int i = 0; i < (int)ResourceType.MAX; i++)
        {
            mTotalResources.Add((ResourceType)i, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            RegenerateNavSurface();

        // Check for upgrade availability
        if (mTotalResources[(int)ResourceType.WOOD] >= 50)
        {
            mSpeedIncrease.interactable = true;
        }
    }

    // Public Functions
    public void SetClickedObject(GameObject selectedObject, Sprite icon = null)
    {
        if (selectedObject != null)
        {
            if (selectedObject.GetComponent<Human>() != null)
            {
                mCurrentHuman = selectedObject.GetComponent<Human>();
                mCurrentSelectedIcon.sprite = icon;
                mCurrentSelectedIcon.color = mCurrentHuman.IconColor;
                mCurrentSelectedIcon.gameObject.SetActive(true);
                //SetHitPointsUI(mCurrentHuman.HitPoints);
            }
            else if (selectedObject.GetComponent<Resource>() != null && mCurrentHuman != null)
            {
                mCurrentHuman.Target = selectedObject.GetComponent<Resource>();
            }
            else
            {
                mCurrentHuman = null;
                mCurrentSelectedIcon = null;
                //mCurrentTree = null;
            }
        }
    }

    public void RegenerateNavSurface()
    {
        if (mGroundSurface != null)
        {
            mGroundSurface.BuildNavMesh();
            //Debug.Log("Regenerated");
        }
    }

    public GameObject GetHome() {  return mHome; }

    public void ChoppedTree(Resource collectedResource)
    {
        if (collectedResource != null)
        {
            mTrees.Remove(collectedResource.gameObject);
        }
    }
    public void IncrementResource(Dictionary<ResourceType, float> returnResources)
    {
        // Pass over returned resources
        for (int i = 0; i < (int)ResourceType.MAX; i++)
        {
            mTotalResources[(ResourceType)i] += returnResources[(ResourceType)i];


            // Get rid of this once I have all the UI set up
            // Bandaid to stop out of bounds error
            if (i < mResourceCountUI.Count)
            {
                mResourceCountUI[i].text = mTotalResources[(ResourceType)i].ToString();
            }
        }

        //mResourceCountUI[0].text = mTotalResources[0].ToString();

        // Check if updgrades are now avialable
    }

    public void SetHitPointsUI(float value)
    {
        mSelectedHitPoints.text = $"Current hitpoints: {value}";
    }

    // Private Functions
    private IEnumerator RegenerateDelay(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public void IncreaseSpeedUpgrade()
    {
        // Deduct improvement
        mTotalResources[(int)ResourceType.WOOD] -= 50;

        // Check if it is still available
        if (mTotalResources[(int)ResourceType.WOOD] < 50)
        {
            mSpeedIncrease.interactable = false;
        }

        // Apply increase
        for (int i = 0; i < mHumans.Count; i++)
        {
            mHumans[i].GetComponent<Human>().Speed *= 1.5f;
        }

        mResourceCountUI[(int)ResourceType.WOOD].text = mTotalResources[(int)ResourceType.WOOD].ToString();
    }

    private Vector3 GetFreePosition(Bounds spawnBounds)
    {
        float x = spawnBounds.extents.x;
        float z = spawnBounds.extents.z;
        // To stop resources spawning on top of each other
        //  First we give it a position
        Vector3 newPos = new Vector3(UnityEngine.Random.Range(-x, x), 1, UnityEngine.Random.Range(-z, z));

        // Check again all the current existing resource positions
        for (int y = 0; y < mResourcePositions.Count; y++)
        {
            float dist = Vector3.Distance(newPos, mResourcePositions[y]);
            // While the distance is less than desired
            while (dist < mSpawnDistance)
            {
                // Keep looking for a new position till one is found
                newPos = new Vector3(UnityEngine.Random.Range(-x, x), 1, UnityEngine.Random.Range(-z, z));
                dist = Vector3.Distance(newPos, mResourcePositions[y]);
            }
        }
        mResourcePositions.Add(newPos);
        // Once a valid position has been found, then it can be set for the instance
        return newPos;
    }
}
