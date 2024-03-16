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

    protected float mHitPoints = 0.0f;
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

    public virtual void Interact(float damage)
    {
        mHitPoints -= damage;
        
        if (mHitPoints < 0.0f)
        {
            // Destroyed
        }
    }

    public virtual void OnMouseDown()
    {
        GameManager.Instance.SetClickedObject(gameObject);
    }
}
