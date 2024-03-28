using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TownCentre : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            Human tmp = other.gameObject.GetComponent<Human>();
            if (tmp != null)
            {
                if (this == tmp.HeadingHome)
                {
                    tmp.Arrived = true;
                    GameManager.Instance.IncrementResource(tmp.CurrentResources);
                    tmp.Arrived = true;
                }
            }
        }
    }
}
