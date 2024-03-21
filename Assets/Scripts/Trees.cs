using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : Resource
{
    public override void Init(ResourceType type, float hitPoints, float returnAmount)
    {
        base.Init(type, hitPoints, returnAmount);
    }

    public override void Tick()
    {
        base.Tick();
    }

    public override float Interact(float damage, out float resourceGainAmount)
    {
        resourceGainAmount = mReturnResource;
        mHitPoints -= damage;

        return mHitPoints;
    }
}
