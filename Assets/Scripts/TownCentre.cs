using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TownCentre : MonoBehaviour
{
    // Resources Count
    private float mWoodCount, mOreCount, mCoalCount, mMeatCount = 0;

    public TMP_Text mUIWoodCount, mUIOreCount, mUICoalCount, mUIMeatCount;
    public void TransfereResource(Resource type, float resourceAmount)
    {
        switch (type.Type)
        {
            case ResourceType.WOOD:
                mWoodCount += resourceAmount;
                mUIWoodCount.text = mWoodCount.ToString();
                break;
            case ResourceType.ORE:
                mOreCount += resourceAmount;
                mUIOreCount.text = mOreCount.ToString();
                break;
            case ResourceType.COAL:
                mCoalCount += resourceAmount;
                mUICoalCount.text = mCoalCount.ToString();
                break;
            case ResourceType.MEAT:
                mMeatCount += resourceAmount;
                mUIMeatCount.text = mMeatCount.ToString();
                break;
            case ResourceType.NONE:
                break;
            default:
                break;
        }
    }
}
