using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : Resource
{
    public override void Init(ResourceType type, float hitPoints)
    {
        base.Init(type, hitPoints);
    }
    public override void Tick()
    {
        base.Tick();
        Debug.Log($"I am a {mType}");
    }
}
