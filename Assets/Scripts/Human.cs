using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    // Public vars
    public Sprite mIcon = null;
    public float mInteractDamage = 25.0f;
    public float mInteractionDistance = 1.5f;
    [SerializeField]
    private float mHitPoints = 100.0f;
    public float HitPoints
    {
        get { return mHitPoints; }
        set { mHitPoints = value; }
    }
    public LayerMask mRaycastHitTargets;
    public ParticleSystem mInteractParticle;
    //Private vars
    private NavMeshAgent mAgent = null;
    public NavMeshAgent Agent
    {
        get { return mAgent; }
    }
    private Resource mTarget = null;
    public Resource Target
    {
        set
        {
            if (value != null)
            {
                mTarget = value;

                mAgent.SetDestination(mTarget.transform.position);

                mArrived = false;
            }
        }

        get { return mTarget; }
    }
    private bool mArrived = false;
    private Rigidbody mRigidbody = null;
    public Rigidbody Rigidbody
    {
        get { return mRigidbody; }
    }
    public bool Arrived
    {
        set
        {
            if (value)
            {
                // Going to try stop the Humans bouncing when near their targets and to try improve movement.
                mAgent.destination = transform.position;
                mRigidbody.velocity = Vector3.zero;
            }
            mArrived = value;
        }
        get { return mArrived; }
    }
    private float mInteractTimer = 0.0f;
    private Transform mHomeTrans = null;
    private bool mHeadingHome = false;
    public bool HeadingHome
    {
        get { return mHeadingHome; }
        set { mHeadingHome = value; }
    }
    private Color mIconColor = Color.white;
    public Color IconColor
    {
        set { mIconColor = value; }
        get { return mIconColor; }
    }
    private Dictionary<ResourceType, float> mCurrentResources = new Dictionary<ResourceType, float>();
    public Dictionary<ResourceType, float> CurrentResources
    {
        get
        {
            // Have to create another dictionary cause Unity dumb
            Dictionary<ResourceType, float> tmp = new Dictionary<ResourceType, float>();

            // empty the humans carrying resources when tehy return home.
            // May have to break this into its own function if I just want to get values without zeroing out
            for (int i = 0; i < mCurrentResources.Count; i++)
            {
                tmp.Add((ResourceType)i, mCurrentResources[(ResourceType)i]);
                mCurrentResources[(ResourceType)i] = 0;
            }

            return tmp;
        }
    }
    [SerializeField]
    private float mSpeed = 10.0f;
    public float Speed
    {
        set
        {
            mSpeed = value;

            if (mAgent != null)
            {
                mAgent.speed = mSpeed;
            }
        }
        get { return mSpeed; }
    }
    [SerializeField]
    private float mInteractTime = 1.0f;
    public float InteractionTime
    {
        get { return mInteractTime; }
        set { mInteractTime = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        mInteractTimer = mInteractTime;

        mHomeTrans = GameManager.Instance.GetHome().transform;

        // Set up resource dictionary
        for (int i = 0; i < (int)ResourceType.MAX; i++)
        {
            mCurrentResources.Add((ResourceType)i, 0.0f);
        }

        mRigidbody = GetComponent<Rigidbody>();
        if (mRigidbody == null)
        {
            Debug.Log($"Missing RB on {gameObject.name}");
        }

        mAgent = GetComponent<NavMeshAgent>();

        mSpeed = mAgent.speed;

        if (mAgent == null)
        {
            Debug.Log($"Missing agent on {name}");
        }

        mAgent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mArrived && mTarget != null)
        {
            mInteractTimer -= Time.deltaTime;

            if (mInteractTimer <= 0.0f)
            {
                switch (mTarget.Type)
                {
                    case ResourceType.WOOD:
                        Trees tree = mTarget.GetComponent<Trees>();
                        if (tree.Interact(mInteractDamage, out float resources) < 0)
                        {
                            ResourceCollected();
                        }
                        mCurrentResources[ResourceType.WOOD] += resources;
                        break;
                    case ResourceType.ORE:
                        Ore ore = mTarget.GetComponent<Ore>();
                        if (ore.Interact(mInteractDamage, out float resource) < 0)
                        {
                            ResourceCollected();
                        }
                        mCurrentResources[ResourceType.ORE] += resource;
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
                mInteractTimer = mInteractTime;
                mInteractParticle.Play();
            }
        }
    }

    // Public functions

    // Private functions
    private void OnMouseDown()
    {
        GameManager.Instance.SetClickedObject(gameObject, mIcon);

        if (mAgent.enabled == false)
            mAgent.enabled = true;
    }

    // Boiler plater code
    private void ResourceCollected()
    {
        mTarget.Collected();
        mTarget = null;
        // I will raycast to the house to set the home position so that the human will go to that spot rather than trying to all get to the same spot
        Vector3 dir = mHomeTrans.position - transform.position;
        Physics.Raycast(transform.position, dir, out RaycastHit hitInfo, float.MaxValue, mRaycastHitTargets);
        if (hitInfo.collider.gameObject != null)
        {
            // Now move towards that point on the home
            mAgent.SetDestination(hitInfo.point);
        }
        else
        {
            // Just head towards the house if the raycast fails
            mAgent.SetDestination(mHomeTrans.position);
        }
        // Set these since we will always move towards the house
        mArrived = false;
        mHeadingHome = true;
    }
}
