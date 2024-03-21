using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    // Public vars
    public Sprite mIcon = null;
    public float mInteractTime = 1.0f;
    public float mInteractDamage = 25.0f;
    public float mInteractionDistance = 1.5f;
    public float mHitPoints = 100.0f;
    // Private vars
    private NavMeshAgent mAgent = null;
    private Resource mTarget = null;
    private bool mArrived = false;
    private float mInteractTimer = 0.0f;
    private Transform mHomeTrans = null;
    private bool mHeadingHome = false;
    private float mResourceCount = 0;

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
                            mAgent.SetDestination(mHomeTrans.position);
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
            }
        }
    }

    // Public functions
    public void SetTarget(Resource target)
    {
        if (target != null)
        {
            mTarget = target;

            mAgent.SetDestination(mTarget.transform.position);

            mArrived = false;
        }
    }

    public float GetHitPoints()
    {
        return mHitPoints;
    }

    public Resource GetTarget() { return mTarget; }
    public bool GetHeadingHome() {  return mHeadingHome; }

    public float GetResources()
    {
        float resourceAmount = mResourceCount;
        mResourceCount = 0;
        return resourceAmount;
    }

    public void SetArrived(bool arrived) { mArrived = arrived; }
    public void SetHomeArrivedHome(bool arrivedHome) {  mHeadingHome = arrivedHome; }
    // Private functions
    private void OnMouseDown()
    {
        GameManager.Instance.SetClickedObject(gameObject, mIcon);
    }
}
