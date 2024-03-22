using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public enum ResourceType
{
    WOOD,
    ORE,
    COAL,
    MEAT,
    NONE
}
public class GameManager : MonoBehaviour
{
    // Public Vars
    // Spawn Values
    public int mMaxTreeSpawn = 100;
    public int mMaxHumanSpawn = 1;
    // Prefabs
    public GameObject mGround = null;
    public GameObject mTree = null;
    public GameObject mHuman = null;
    // UI
    public Image mCurrentSelectedIcon = null;
    public TMP_Text mSelectedHitPoints = null;
    public TMP_Text mWoodCountUI = null;
    // Game objects
    public GameObject mHome = null;
    // Private Vars
    private List<GameObject> mTrees = new List<GameObject>();
    private List<GameObject> mHumans = new List<GameObject>();
    private Human mCurrentHuman = null;
    private NavMeshSurface mGroundSurface = null;
    private float mWoodCount = 0;

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

                float halfX = x / 2;
                float halfZ = z / 2;
                for (int i = 0; i < mMaxTreeSpawn; i++)
                {
                    float posX = UnityEngine.Random.Range(-x, x);
                    float posZ = UnityEngine.Random.Range(-z, z);
                    GameObject tmp = Instantiate(mTree, new Vector3(posX, 1, posZ), transform.rotation);

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
                    GameObject tmp = Instantiate(mHuman, new Vector3(posX, 1, posZ), transform.rotation);

                    tmp.name = $"Human{i}";
                    mHumans.Add(tmp);
                }
                //--
            }

            if (mCurrentSelectedIcon != null)
            {
                mCurrentSelectedIcon.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            RegenerateNavSurface();
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
                mCurrentSelectedIcon.gameObject.SetActive(true);
                SetHitPointsUI(mCurrentHuman.GetHitPoints());
            }
            else if (selectedObject.GetComponent<Resource>() != null && mCurrentHuman != null)
            {
                mCurrentHuman.SetTarget(selectedObject.GetComponent<Resource>());
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
            Debug.Log("Regenerated");
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

    public void IncrementWoodResourceUICount(float amount)
    {
        mWoodCount += amount;
        mWoodCountUI.text = mWoodCount.ToString();
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
}
