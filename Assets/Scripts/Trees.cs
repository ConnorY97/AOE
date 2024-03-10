using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : MonoBehaviour
{
    private float mHitPoints = 100.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        GameManager.Instance.SetClickedObject(gameObject);
    }

    // Public Functions
    public bool Chop(float damage)
    {
        mHitPoints -= mHitPoints;

        if (mHitPoints <= 0)
        {
            GameManager.Instance.RegenerateNavSurface();
            this.gameObject.SetActive(false);

            return false;
        }

        return true;
    }

    public float GetHitPoints()
    {
        return mHitPoints;
    }
}
