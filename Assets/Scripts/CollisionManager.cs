using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag(other.gameObject.tag))
        {
            other.gameObject.GetComponent<Collider>().enabled = false;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;

            Bounds bounds = gameObject.GetComponent<Collider>().bounds;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(other.transform.DOMoveY(other.transform.position.y + bounds.extents.y, 1f));
            sequence.Append(other.transform.DOMove(new Vector3(bounds.center.x, transform.position.y, bounds.center.z), 1f));
            sequence.Append(other.transform.DOScale(new Vector3(0, 0, 0), 1f));
            sequence.OnComplete(() => other.gameObject.SetActive(false));

            VirtualAssistantManager.Instance.Correct();
        }
        else if (!other.gameObject.CompareTag("Untagged"))
        {
            VirtualAssistantManager.Instance.Wrong();
        }
    }
}
