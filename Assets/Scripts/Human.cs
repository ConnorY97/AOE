using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    public Sprite mIcon = null;
    public float mInteractTime = 1.0f;
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
    public bool Arrived
    {
        set { mArrived = value; }
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
    private float mResourceCount = 0;
    private Color mIconColor = Color.white;
    public Color IconColor
    {
        set { mIconColor = value; }
        get { return mIconColor; }
    }

    // Start is called before the first frame update
    void Start()
    {
        mAgent = GetComponent<NavMeshAgent>();

        if (mAgent == null)
        {
            Debug.Log($"Missing agent on {name}");
        }

        mInteractTimer = mInteractTime;

        mHomeTrans = GameManager.Instance.GetHome().transform;
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
                        Trees tmp = mTarget.GetComponent<Trees>();
                        if (tmp.Interact(mInteractDamage, out float resources) < 0)
                        {
                            mResourceCount += resources;
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
                        break;
                    case ResourceType.ORE:
                        break;
                    case ResourceType.COAL:
                        break;
                    case ResourceType.MEAT:
                        break;
                    case ResourceType.NONE:
                        break;
                    default:
                        break;
                }
                mInteractTimer = mInteractTime;
            }
        }
    }

    // Public functions
    public float GetResources()
    {
        float resourceAmount = mResourceCount;
        mResourceCount = 0;
        return resourceAmount;
    }
    // Private functions
    private void OnMouseDown()
    {
        GameManager.Instance.SetClickedObject(gameObject, mIcon);
    }
}
