using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    private NavMeshAgent mAgent = null;
    private bool mSelected = false;

    private GameObject mTarget = null;
    // Start is called before the first frame update
    void Start()
    {
        mAgent = GetComponent<NavMeshAgent>();

        if (mAgent == null)
        {
            Debug.Log($"Missing agent on {name}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Publi functions 
    public void SetTarget(GameObject target)
    {
        if (target != null)
        {
            mTarget = target;

            mAgent.SetDestination(mTarget.transform.position);
        }
    }
    // Private functions
    private void OnMouseDown()
    {
        GameManager.Instance.SetClickedObject(gameObject);
    }


}
