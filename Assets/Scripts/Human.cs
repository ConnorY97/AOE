using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Human : MonoBehaviour
{
    // Public vars
    public Sprite mIcon = null;
    public float mChopTime = 1.0f;
    public float mChopDamage = 25.0f;
    public float mHitPoints = 100.0f;
    // Private vars
    private NavMeshAgent mAgent = null;
    private GameObject mTarget = null;
    private bool mArrived = false;
    private float mChopTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        mAgent = GetComponent<NavMeshAgent>();

        if (mAgent == null)
        {
            Debug.Log($"Missing agent on {name}");
        }

        mChopTimer = mChopTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mArrived && mTarget != null)
        {
            float dist = Vector3.Distance(transform.position, mTarget.transform.position);
            //Debug.Log(dist);

            if (dist < 1.5f)
            {
                mArrived = true;
            }
        }

        if (mArrived && mTarget != null)
        {
            mChopTimer -= Time.deltaTime;

            if (mChopTimer <= 0.0f)
            {
                if (mTarget.GetComponent<Trees>().Chop(mChopDamage))
                {
                    mChopTimer = mChopTime;
                }
                else
                {
                    mTarget = null;

                    mArrived = false;

                    mAgent.SetDestination(GameManager.Instance.GetHome().transform.position);
                }
            }
        }
    }

    // Publi functions
    public void SetTarget(GameObject target)
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
    // Private functions
    private void OnMouseDown()
    {
        GameManager.Instance.SetClickedObject(gameObject, mIcon);
    }
}
