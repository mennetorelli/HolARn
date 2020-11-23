using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag(other.gameObject.tag))
        {
            other.gameObject.SetActive(false); 
            VirtualAssistantManager.Instance.Correct();
        }
        else
        {
            VirtualAssistantManager.Instance.Wrong();
        }
    }
}
