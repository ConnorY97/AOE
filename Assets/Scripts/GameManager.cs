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
    // Public Variables
    public int mMaxHumanSpawn = 1;
    public int mMaxTreeSpawn = 100;
    public int mMaxOreSpawn = 50;
    public float mSpawnDistance = 3;

    public GameObject mGround = null;
    public List<GameObject> mTrees = new List<GameObject>();
    public GameObject mHuman = null;
    public GameObject mOre = null;
    public GameObject mHome = null;

    public Image mCurrentSelectedIcon = null;
    public TMP_Text mSelectedHitPoints = null;
    public List<TMP_Text> mResourceCountUI = new List<TMP_Text>();
    public Button mSpeedIncrease = null;
    public Button mInteractionIncrease = null;
    public Button mSpawnHuman = null;

    // Private Variables
    private List<GameObject> mObjects = new List<GameObject>();
    private List<GameObject> mHumans = new List<GameObject>();
    public List<GameObject> Humans
    {
        get { return mHumans; }
    }
    private Human mCurrentHuman = null;
    public Human CurrentHuman
    {
        get { return mCurrentHuman; }
    }
    private NavMeshSurface mGroundSurface = null;
    private Dictionary<ResourceType, float> mTotalResources = new Dictionary<ResourceType, float>();
    public Dictionary<ResourceType, float> TotalResources
    {
        get { return mTotalResources; }
    }
    private List<Vector3> mResourcePositions = new List<Vector3>();
    private MeshCollider mGroundMesh = null;
    private Bounds mBounds = new Bounds();

    // Singleton Instance
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        mResourcePositions.Add(mHome.transform.position);

        if (mGround != null)
        {
            InitializeGround();
            InitializeNavMesh();
            SpawnInitialHumans();
            SpawnResources();
            DisableUIIcons();
            InitializeResourceDictionary();
        }

        UpdateUpgradeButtons();
    }

    private void InitializeGround()
    {
        mGroundMesh = mGround.GetComponent<MeshCollider>();
        if (mGroundMesh != null)
        {
            mBounds = mGroundMesh.bounds;
        }
    }

    private void InitializeNavMesh()
    {
        mGroundSurface = mGround.GetComponent<NavMeshSurface>();
        if (mGroundSurface != null)
        {
            mGroundSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogError("Failed to create NavMesh surface.");
        }
    }

    private void SpawnInitialHumans()
    {
        for (int i = 0; i < mMaxHumanSpawn; i++)
        {
            Vector3 spawnPosition = GetRandomPositionWithinBounds();
            SpawnHuman(spawnPosition, i);
        }
        StartCoroutine(PlaceObjectsOnMap(mHumans, 0.5f));
    }

    private void SpawnResources()
    {
        for (int i = 0; i < mMaxTreeSpawn; i++)
        {
            int rand = UnityEngine.Random.Range(0, mTrees.Count - 1);
            GameObject tree = Instantiate(mTrees[rand], Vector3.one * 100, Quaternion.identity);
            tree.name = $"Tree{i}";
            tree.GetComponent<Trees>().Init(ResourceType.WOOD, 100f, 10f);
            mObjects.Add(tree);
        }

        for (int i = 0; i < mMaxOreSpawn; i++)
        {
            GameObject ore = Instantiate(mOre, Vector3.one * 100, Quaternion.identity);
            ore.name = $"Ore{i}";
            ore.GetComponent<Ore>().Init(ResourceType.ORE, 100f, 10f);
            mObjects.Add(ore);
        }

        StartCoroutine(PlaceObjectsOnMap(mObjects, 0.0f));

        // Build the nav mesh after all the objects have been placed
        mGroundSurface.BuildNavMesh();
    }

    private void DisableUIIcons()
    {
        if (mCurrentSelectedIcon != null)
        {
            mCurrentSelectedIcon.gameObject.SetActive(false);
        }

        mSpeedIncrease.interactable = false;
    }

    private void InitializeResourceDictionary()
    {
        for (int i = 0; i < (int)ResourceType.MAX; i++)
        {
            mTotalResources.Add((ResourceType)i, 0f);
        }
    }

    private void Update()
    {
        // Update logic if needed
    }

    public void SetClickedObject(GameObject selectedObject, Sprite icon = null)
    {
        if (selectedObject != null)
        {
            var human = selectedObject.GetComponent<Human>();
            var resource = selectedObject.GetComponent<Resource>();

            if (human != null)
            {
                mCurrentHuman = human;
                UpdateSelectedIcon(icon, human.IconColor);
            }
            else if (resource != null && mCurrentHuman != null)
            {
                mCurrentHuman.Target = resource;
            }
            else
            {
                mCurrentHuman = null;
                mCurrentSelectedIcon.gameObject.SetActive(false);
            }
        }
    }

    public void RegenerateNavSurface()
    {
        mGroundSurface?.BuildNavMesh();
    }

    public GameObject GetHome() => mHome;

    public void IncrementResource(Dictionary<ResourceType, float> returnResources)
    {
        foreach (var resource in returnResources)
        {
            mTotalResources[resource.Key] += resource.Value;
            UpdateResourceUI(resource.Key);
        }

        UpdateUpgradeButtons();
    }

    public void SetHitPointsUI(float value)
    {
        mSelectedHitPoints.text = $"Current hitpoints: {value}";
    }

    public void IncreaseSpeedUpgrade()
    {
        ApplyUpgrade(new List<ResourceType> { ResourceType.WOOD }, 50f);
    }

    public void IncreaseInteractionUpgrade()
    {
        ApplyUpgrade(new List<ResourceType> { ResourceType.ORE }, 50f);
    }

    public void SpawnHumanUpgrade()
    {
        ApplyUpgrade(new List<ResourceType> { ResourceType.WOOD, ResourceType.ORE }, 100f);
    }

    private void ApplyUpgrade(List<ResourceType> resourceTypes, float deductionAmount)
    {
        foreach (var resourceType in resourceTypes)
        {
            mTotalResources[resourceType] -= deductionAmount;
            UpdateResourceUI(resourceType);
        }

        if (resourceTypes.Count == 1)
        {
            ApplySingleUpgrade(resourceTypes[0]);
        }
        else if (resourceTypes.Count == 2)
        {
            SpawnHuman(GetFreePosition(0f), mHumans.Count);
        }

        UpdateUpgradeButtons();
    }

    private void ApplySingleUpgrade(ResourceType resourceType)
    {
        foreach (var human in mHumans)
        {
            var humanComponent = human.GetComponent<Human>();
            if (resourceType == ResourceType.WOOD)
            {
                humanComponent.Speed *= 1.5f;
            }
            else if (resourceType == ResourceType.ORE)
            {
                humanComponent.InteractionTime -= 0.05f;
            }
        }
    }

    private void UpdateResourceUI(ResourceType resourceType)
    {
        if ((int)resourceType < mResourceCountUI.Count)
        {
            if (mResourceCountUI[(int)resourceType] != null)
            {
                int index = (int)resourceType;
                if (index < mResourceCountUI.Count)
                {
                    mResourceCountUI[index].text = mTotalResources[resourceType].ToString();
                }
            }
        }
    }

    private void UpdateUpgradeButtons()
    {
        mSpeedIncrease.interactable = mTotalResources[ResourceType.WOOD] >= 50;
        mInteractionIncrease.interactable = mTotalResources[ResourceType.ORE] >= 50;
        mSpawnHuman.interactable = mTotalResources[ResourceType.WOOD] >= 100 && mTotalResources[ResourceType.ORE] >= 100;
    }

    private Vector3 GetFreePosition(float yHeight)
    {
        Vector3 newPos = GetRandomPositionWithinBounds(yHeight);

        while (Physics.OverlapSphere(newPos, mSpawnDistance).Length > 1)
        {
            newPos = GetRandomPositionWithinBounds(yHeight);
        }

        return newPos;
    }

    private Vector3 GetRandomPositionWithinBounds(float yHeight = 0f)
    {
        float x = mBounds.extents.x;
        float z = mBounds.extents.z;
        return new Vector3(UnityEngine.Random.Range(-x, x), yHeight, UnityEngine.Random.Range(-z, z));
    }

    private IEnumerator PlaceObjectsOnMap(List<GameObject> objectsToPlace, float yHeight)
    {
        foreach (var obj in objectsToPlace)
        {
            obj.transform.position = GetFreePosition(yHeight);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void SpawnHuman(Vector3 newPos, int humanNumber)
    {
        var humanObj = Instantiate(mHuman, newPos, Quaternion.identity);
        humanObj.name = $"Human{humanNumber}";

        var humanComponent = humanObj.GetComponent<Human>();
        if (humanComponent != null)
        {
            var uniqueColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
            humanComponent.IconColor = uniqueColor;
            if (humanComponent.Rigidbody != null)
            {
                humanComponent.Rigidbody.velocity = Vector3.zero;
            }
            humanObj.GetComponent<Renderer>().material.color = uniqueColor;
        }

        mHumans.Add(humanObj);
    }

    private void UpdateSelectedIcon(Sprite icon, Color color)
    {
        mCurrentSelectedIcon.sprite = icon;
        mCurrentSelectedIcon.color = color;
        mCurrentSelectedIcon.gameObject.SetActive(true);
    }
}
