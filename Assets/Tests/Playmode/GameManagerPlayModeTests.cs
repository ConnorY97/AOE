//using System.Collections;
//using System.Collections.Generic;
//using NUnit.Framework;
//using UnityEngine;
//using UnityEngine.TestTools;
//using UnityEngine.UI;
//using TMPro;
//using Unity.AI.Navigation;
//using UnityEngine.AI;

//public class GameManagerPlayModeTests
//{
//    private GameObject gameManagerObject;
//    private GameManager gameManager;

//    [SetUp]
//    public void SetUp()
//    {
//        // Create a new GameObject and add the GameManager component
//        gameManagerObject = new GameObject();
//        gameManager = gameManagerObject.AddComponent<GameManager>();

//        // Set up necessary GameObjects and UI elements for testing
//        gameManager.mGround = new GameObject("Ground");
//        gameManager.mGround.AddComponent<MeshCollider>();
//        gameManager.mGround.AddComponent<NavMeshSurface>();

//        gameManager.mHome = new GameObject("Home");

//        gameManager.mTree = new GameObject("Tree");
//        gameManager.mTree.AddComponent<Trees>();

//        gameManager.mHuman = new GameObject("Human");
//        gameManager.mHuman.AddComponent<MeshFilter>();
//        Assert.IsNotNull(gameManager.mHuman.AddComponent<MeshRenderer>(), "Failed to add renderer in set up");
//        gameManager.mHuman.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
//        gameManager.mHuman.AddComponent<Human>();
//        gameManager.mHuman.AddComponent<Rigidbody>();
//        gameManager.mHuman.AddComponent<NavMeshAgent>();

//        gameManager.mOre = new GameObject("Ore");
//        gameManager.mOre.AddComponent<Ore>();

//        gameManager.mCurrentSelectedIcon = new GameObject("SelectedIcon").AddComponent<Image>();
//        gameManager.mSelectedHitPoints = new GameObject("SelectedHitPoints").AddComponent<TMP_Text>();

//        gameManager.mSpeedIncrease = new GameObject("SpeedIncrease").AddComponent<Button>();
//        gameManager.mInteractionIncrease = new GameObject("InteractionIncrease").AddComponent<Button>();
//        gameManager.mSpawnHuman = new GameObject("SpawnHuman").AddComponent<Button>();

//        // Initialize UI resource count list
//        for (int i = 0; i < (int)ResourceType.MAX; i++)
//        {
//            gameManager.mResourceCountUI.Add(new GameObject($"ResourceCountUI{i}").AddComponent<TMP_Text>());
//        }
//    }

//    [TearDown]
//    public void TearDown()
//    {
//        // Clean up the created GameObjects after each test
//        Object.Destroy(gameManagerObject);
//        Object.Destroy(gameManager.mGround);
//        Object.Destroy(gameManager.mHome);
//        Object.Destroy(gameManager.mTree);
//        Object.Destroy(gameManager.mHuman);
//        Object.Destroy(gameManager.mOre);
//    }

//    [UnityTest]
//    public IEnumerator TestHumanSpawn()
//    {
//        // Set parameters for spawning
//        gameManager.mMaxHumanSpawn = 1;
//        gameManager.mMaxTreeSpawn = 0;
//        gameManager.mMaxOreSpawn = 0;

//        // Call Start method to initialize and spawn objects
//        //gameManager.InitializeGame();

//        // Wait for the next frame to ensure objects are spawned
//        yield return null;

//        // Check if a human has been spawned
//        Assert.AreEqual(1, gameManager.Humans.Count);
//        Assert.IsNotNull(gameManager.Humans[0]);
//    }

//    [UnityTest]
//    public IEnumerator TestResourceIncrement()
//    {
//        // Initialize resource dictionary
//        //gameManager.InitializeGame();

//        // Create a dictionary to increment resources
//        var incrementResources = new Dictionary<ResourceType, float>
//        {
//            { ResourceType.WOOD, 10.0f },
//            { ResourceType.ORE, 5.0f },
//            { ResourceType.COAL, 0.0f },
//            { ResourceType.MEAT, 0.0f }
//        };

//        // Call IncrementResource method
//        gameManager.IncrementResource(incrementResources);

//        // Wait for the next frame to ensure UI updates
//        yield return null;

//        // Check if resources have been incremented correctly
//        Assert.AreEqual(10.0f, gameManager.TotalResources[ResourceType.WOOD]);
//        Assert.AreEqual(5.0f, gameManager.TotalResources[ResourceType.ORE]);
//        Assert.AreEqual("10", gameManager.mResourceCountUI[(int)ResourceType.WOOD].text);
//        Assert.AreEqual("5", gameManager.mResourceCountUI[(int)ResourceType.ORE].text);
//    }

//    [UnityTest]
//    public IEnumerator TestSetClickedObject()
//    {
//        // Initialize GameManager
//        //gameManager.InitializeGame();

//        // Create a new human GameObject
//        var humanObject = new GameObject("TestHuman");
//        humanObject.AddComponent<Human>();
//        var renderer = humanObject.AddComponent<Renderer>();
//        renderer.material = new Material(Shader.Find("Standard"));
//        humanObject.GetComponent<Human>().IconColor = Color.red;

//        // Call SetClickedObject with the human GameObject
//        gameManager.SetClickedObject(humanObject);

//        // Wait for the next frame to ensure UI updates
//        yield return null;

//        // Check if the current human and icon have been set correctly
//        Assert.AreEqual(humanObject.GetComponent<Human>(), gameManager.CurrentHuman);
//        Assert.AreEqual(Color.red, gameManager.mCurrentSelectedIcon.color);
//        Assert.AreEqual(true, gameManager.mCurrentSelectedIcon.gameObject.activeSelf);
//    }
//}
