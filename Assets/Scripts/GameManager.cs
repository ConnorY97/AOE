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
    public GameObject mSpawnCheckObject = null;
    // UI
    public Image mCurrentSelectedIcon = null;
    public TMP_Text mSelectedHitPoints = null;
    public List<TMP_Text> mResourceCountUI = new List<TMP_Text>();
    public Button mSpeedIncrease = null;
    public Button mInteractionIncrease = null;
    // Game objects
    public GameObject mHome = null;
    // Private Vars
    private List<GameObject> mObjects = new List<GameObject>();
    private List<GameObject> mHumans = new List<GameObject>();
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
                    //mResourcePositions.Add(newPos);

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

                // Spawn the humans
                StartCoroutine(PlaceObjects(mHumans, 0.5f));
                //--
                // Tree spawning
                for (int i = 0; i < mMaxTreeSpawn; i++)
                {
                    GameObject tmp = Instantiate(mTree, new Vector3(100.0f, 100.0f, 100.0f), transform.rotation);

                    tmp.name = $"Tree{i}";

                    tmp.GetComponent<Trees>().Init(ResourceType.WOOD, 100.0f, 10.0f);

                    mObjects.Add(tmp);
                }
                // --
                // Ore spawning
                for (int i = 0; i < mMaxOreSpawn; i++)
                {
                    GameObject tmp = Instantiate(mOre, new Vector3(100.0f, 100.0f, 100.0f), transform.rotation);

                    tmp.name = $"Ore{i}";

                    tmp.GetComponent<Ore>().Init(ResourceType.ORE, 100.0f, 10.0f);

                    mObjects.Add(tmp);
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

        // Now move all the objects onto the map and make sure they are not overlapping
        StartCoroutine(PlaceObjects(mObjects, 0.0f));
    }

    private void Update()
    {

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

        CheckUpgradeAvailability();
    }

    public void SetHitPointsUI(float value)
    {
        mSelectedHitPoints.text = $"Current hitpoints: {value}";
    }

    // Private Functions
    public void IncreaseSpeedUpgrade()
    {
        ApplyUpdgrade(ResourceType.WOOD);
    }

    public void IncreaseInteractionUpgrade()
    {
        ApplyUpdgrade(ResourceType.ORE);
    }

    private void ApplyUpdgrade(ResourceType type)
    {
        // Deduct improvement
        mTotalResources[type] -= 50;

        // Apply increase
        for (int i = 0; i < mHumans.Count; i++)
        {
            switch (type)
            {
                case ResourceType.WOOD:
                    mHumans[i].GetComponent<Human>().Speed *= 1.5f;
                    // Check if it is still available
                    if (mTotalResources[type] < 50)
                    {
                        mSpeedIncrease.interactable = false;
                    }
                    break;
                case ResourceType.ORE:
                    mHumans[i].GetComponent<Human>().InteractionTime -= 0.05f;
                    // Check if it is still available
                    if (mTotalResources[type] < 50)
                    {
                        mInteractionIncrease.interactable = false;
                    }
                    break;
                case ResourceType.COAL:
                    break;
                case ResourceType.MEAT:
                    break;
                case ResourceType.MAX:
                    break;
                default:
                    break;
            }
        }

        mResourceCountUI[(int)type].text = mTotalResources[type].ToString();
    }

    private Vector3 GetFreePosition(Bounds spawnBounds, float yHeight)
    {
        float x = spawnBounds.extents.x;
        float z = spawnBounds.extents.z;
        // To stop resources spawning on top of each other
        //  First we give it a position
        Vector3 newPos = new Vector3(UnityEngine.Random.Range(-x, x), yHeight, UnityEngine.Random.Range(-z, z));

        // Trying a new method for spawning objects.
        // Rather than going through a list of position, I will shpere cast at the spawn location a check if there is anything other than the ground there.
        var checkResult = Physics.OverlapSphere(newPos, mSpawnDistance);

        // Recusrsion keep checkign till we find a free stop.
        // Hopefully this will help with the debugging.
        if (checkResult.Length > 1)
        {
            newPos = GetFreePosition(spawnBounds, yHeight);
        }

        // Assign the free position
        return newPos;
    }

    private void CheckUpgradeAvailability()
    {
        // Check for upgrade availability
        if (mTotalResources[ResourceType.WOOD] >= 50)
        {
            mSpeedIncrease.interactable = true;
        }
        if (mTotalResources[ResourceType.ORE] >= 50)
        {
            mInteractionIncrease.interactable = true;
        }
    }

    IEnumerator PlaceObjects(List<GameObject> objectsToPlace, float yHeight)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.

        for (int i = 0; i < objectsToPlace.Count; i++)
        {
            MeshCollider groundMesh = mGround.GetComponent<MeshCollider>();
            Bounds bounds = groundMesh.bounds;
            objectsToPlace[i].transform.position = GetFreePosition(bounds, yHeight);
            mGroundSurface.BuildNavMesh();
            yield return new WaitForSeconds(0.01f);
        }
    }
}
