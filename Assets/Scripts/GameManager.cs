using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
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
    public int mMaxTreeSpawn = 100;
    public int mMaxHumanSpawn = 1;
    public float mSpawnDistance = 3;
    // Prefabs
    public GameObject mGround = null;
    public GameObject mTree = null;
    public GameObject mHuman = null;
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
    private Human mCurrentHuman = null;
    private NavMeshSurface mGroundSurface = null;
    private Dictionary<ResourceType, float> mTotalResources = new Dictionary<ResourceType, float>();

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

                float halfX = x / 5;
                float halfZ = z / 5;
                for (int i = 0; i < mMaxTreeSpawn; i++)
                {
                    // To stop objects spawning on top of each other
                    //  First we give it a position
                    Vector3 newPos = new Vector3(UnityEngine.Random.Range(-x, x), 1, UnityEngine.Random.Range(-z, z));

                    // Check again all the current existing trees
                    for (int y = 0; y < mTrees.Count; y++)
                    {
                        float dist = Vector3.Distance(newPos, mTrees[y].transform.position);
                        // While the distance is less than desired
                        while (dist < mSpawnDistance)
                        {
                            // Keep looking for a new position till one is found
                            newPos = new Vector3(UnityEngine.Random.Range(-x, x), 1, UnityEngine.Random.Range(-z, z));
                            dist = Vector3.Distance(newPos, mTrees[y].transform.position);
                        }
                    }
                    // Once a valid position has been found, then it can be set for the instance
                    GameObject tmp = Instantiate(mTree, newPos, transform.rotation);

                    tmp.name = $"Tree{i}";

                    tmp.GetComponent<Trees>().Init(ResourceType.WOOD, 100.0f, 10.0f);
                    mTrees.Add(tmp);
                }
                // --

                // Create the nav mesh surface before the human,
                //  Otherwise it gets mad there is not surface
                mGroundSurface = mGround.GetComponent<NavMeshSurface>();

                if (mGroundSurface != null)
                {
                    mGroundSurface.BuildNavMesh();
                }
                //--
                // Human spawning
                for (int i = 0; i < mMaxHumanSpawn; i++)
                {
                    float posX = UnityEngine.Random.Range(-halfX, halfX);
                    float posZ = UnityEngine.Random.Range(-halfZ, halfZ);
                    GameObject tmp = Instantiate(mHuman, new Vector3(posX, 1.25f, posZ), transform.rotation);

                    tmp.name = $"Human{i}";

                    Color uniqueColor = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
                    // Set the unique icon colour for each human
                    Human human = tmp.GetComponent<Human>();
                    if (human != null)
                    {
                        human.IconColor = uniqueColor;
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

    //public void IncrementWoodResourceUICount(float amount)
    //{
    //    mWoodCount += amount;
    //    mWoodCountUI.text = mWoodCount.ToString();
    //}

    public void IncrementResource(Dictionary<ResourceType, float> returnResources)
    {
        // Pass over returned resources
        for (int i = 0; i < (int)ResourceType.MAX; i++)
        {
            mTotalResources[(ResourceType)i] += returnResources[(ResourceType)i];

            // TODO: create UI set stuff
        }

        mResourceCountUI[0].text = mTotalResources[0].ToString();

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
}
