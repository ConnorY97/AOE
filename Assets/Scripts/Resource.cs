using UnityEngine;

public abstract class Resource : MonoBehaviour
{
    protected ResourceType mType = ResourceType.MAX;
    public ResourceType Type
    {
        get { return mType; }
    }

    protected float mHitPoints = 100.0f;
    public float HitPoints
    {
        get { return mHitPoints; }
    }

    protected float mReturnResource = 0.0f;
    public float ReturnResource
    {
        get { return mReturnResource; }
    }

    public virtual void Init(ResourceType type, float hitPoints, float returnAmount)
    {
        mType = type;
        mHitPoints = hitPoints;
        mReturnResource = returnAmount;
    }

    private void Update()
    {
        Tick();
    }

    public virtual void Tick()
    {

    }

    public abstract float Interact(float damage, out float resourceGainAmount);

    public virtual void Collected()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RegenerateNavSurface();
        GameManager.Instance.ChoppedTree(this);
        Destroy(gameObject);
    }

    public virtual void OnMouseDown()
    {
        GameManager.Instance.SetClickedObject(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            Human tmp = other.gameObject.GetComponent<Human>();
            if (tmp != null)
            {
                if (this == tmp.Target)
                {
                    tmp.Arrived = true;
                }
            }
        }
    }
}
