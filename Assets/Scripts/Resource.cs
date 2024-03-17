using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Resource : MonoBehaviour
{
    protected ResourceType mType = ResourceType.NONE;
    public ResourceType Type
    {
        get { return mType; }
    }

    protected float mHitPoints = 100.0f;
    public float HitPoints
    {
        get { return mHitPoints; }
    }

    public virtual void Init(ResourceType type, float hitPoints)
    {
        mType = type;
        mHitPoints = hitPoints;
    }

    private void Update()
    {
        Tick();
    }

    public virtual void Tick()
    {

    }

    public virtual void Interact()
    {
    }

    public virtual float Interact(float damage)
    {
        return 0;
    }

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
}
