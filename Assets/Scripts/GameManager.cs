using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

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
    public int mMaxTreeSpawn = 100;
    public int mMaxHumanSpawn = 1;
    public GameObject mGround = null;
    public GameObject mTree = null;
    public GameObject mHuman = null;
    public Image mCurrentSelectedIcon = null;
    public GameObject mHome = null;
    public TMP_Text mSelectedHitPoints = null;
    // Private Vars
    private List<GameObject> mTrees = new List<GameObject>();
    private List<GameObject> mHumans = new List<GameObject>();
    private Human mCurrentHuman = null;
    private Trees mCurrentTree = null;
    private NavMeshSurface mGroundSurface = null;
    //private GameObject mChoopedTree = null;

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

                    tmp.GetComponent<Trees>().Init(ResourceType.WOOD, 100.0f);
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
                    GameObject tmp = Instantiate(mHuman, new Vector3(3, 1, 0), transform.rotation);

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
            else if (selectedObject.GetComponent<Trees>() != null)
            {
                if (mCurrentTree != null)
                {
                    //mCurrentTree.SetSelected(false);
                    mCurrentTree = null;
                }
                mCurrentTree = selectedObject.GetComponent<Trees>();
                //SetHitPointsUI(mCurrentTree.GetHitPoints());
                if (mCurrentHuman != null)
                {
                    mCurrentHuman.SetTarget(mCurrentTree.gameObject);
                }
                //mCurrentTree.SetSelected(true);
            }
            else
            {
                mCurrentHuman = null;
                mCurrentTree = null;
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

    public void ChoppedTree(GameObject choppedTree)
    {
        if (choppedTree != null)
        {
            mTrees.Remove(choppedTree);

            Destroy(choppedTree);

            StartCoroutine(RegenerateDelay(3.0f));

            RegenerateNavSurface();
        }
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
