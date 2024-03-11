using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : MonoBehaviour
{
    private float mHitPoints = 100.0f;

    private bool mSelected = false;

    // Update is called once per frame
    void Update()
    {
        if (mSelected)
        {
            GameManager.Instance.SetHitPointsUI(mHitPoints);
        }
    }

    private void OnMouseDown()
    {
        GameManager.Instance.SetClickedObject(gameObject);
    }

    // Public Functions
    public bool Chop(float damage)
    {
        mHitPoints -= damage;

        if (mHitPoints <= 0)
        {
            GameManager.Instance.ChoppedTree(gameObject);
            return false;
        }

        return true;
    }

    public void SetSelected(bool selected)
    {
        mSelected = selected;
    }

    public float GetHitPoints()
    {
        return mHitPoints;
    }
}
