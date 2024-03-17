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
    public float mInteractTime = 1.0f;
    public float mInteractDamage = 25.0f;
    public float mHitPoints = 100.0f;
    // Private vars
    private NavMeshAgent mAgent = null;
    private Resource mTarget = null;
    private bool mArrived = false;
    private float mInteractTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        mAgent = GetComponent<NavMeshAgent>();

        if (mAgent == null)
        {
            Debug.Log($"Missing agent on {name}");
        }

        mInteractTimer = mInteractTime;
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
            mInteractTimer -= Time.deltaTime;

            if (mInteractTimer <= 0.0f)
            {
                switch (mTarget.Type)
                {
                    case ResourceType.WOOD:
                        Trees tmp = mTarget.GetComponent<Trees>();
                        if (tmp.Interact(mInteractDamage) < 0)
                        {
                            mTarget.Collected();
                            mTarget = null;
                            mAgent.SetDestination(GameManager.Instance.GetHome().transform.position);
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

    // Publi functions
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
    // Private functions
    private void OnMouseDown()
    {
        GameManager.Instance.SetClickedObject(gameObject, mIcon);
    }
}
