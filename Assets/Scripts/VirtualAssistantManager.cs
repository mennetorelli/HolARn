using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

class VirtualAssistantManager : MonoBehaviour 
{
    public Transform targetObject;
    public bool IsBusy;
    public bool IsDragging;
    public int speed = 3;
    public List<AudioClip> AudioList;

    private AudioSource _audioSource;

    public static VirtualAssistantManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

            _audioSource = GetComponent<AudioSource>();
        }
    }

    public void Play()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Play");
    }

    public void Correct()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Jump");
        _audioSource.PlayOneShot(AudioList[0]);
    }

    public void Wrong()
    {
        gameObject.GetComponent<Animator>().SetTrigger("ShakeHead");
        _audioSource.PlayOneShot(AudioList[1]);
    }

    public void ObjectDragged(GameObject draggedObject)
    {
        targetObject = draggedObject.transform;
        gameObject.GetComponent<Animator>().ResetTrigger("DraggingStopped");
        gameObject.GetComponent<Animator>().SetTrigger("DraggingStarted");
        IsDragging = true;
    }

    public void ObjectDropped()
    {
        gameObject.GetComponent<Animator>().ResetTrigger("DraggingStarted");
        gameObject.GetComponent<Animator>().SetTrigger("DraggingStopped");
        IsDragging = false;
    }

    public void Walk()
    {
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(3f);
        gameObject.GetComponent<Animator>().SetTrigger("Walk");
    }

    public void SetTriggers()
    {
        if (IsDragging)
        {
            gameObject.GetComponent<Animator>().SetTrigger("DraggingStarted");
        }
        else
        {
            gameObject.GetComponent<Animator>().SetTrigger("DraggingStopped");
        }
    }


    public GameObject GetClosestInteractable()
    {
        List<GameObject> correctInteractables = GameManager.Instance.InstantiatedInteractables;
        SortByDistance(correctInteractables);
        return correctInteractables[0];
    }

    public GameObject GetClosestTarget()
    {
        List<GameObject> targets = GameManager.Instance.InstantiatedTargets.Where(x => GameManager.Instance.SelectedObject.CompareTag(x.tag)).ToList();
        SortByDistance(targets);
        return targets[0];
    }

    protected void SortByDistance(List<GameObject> items)
    {
        GameObject temp;
        for (int i = 0; i < items.Count; i++)
        {
            for (int j = 0; j < items.Count - 1; j++)
            {
                if (Vector3.Distance(items.ElementAt(j).transform.position, transform.position)
                    > Vector3.Distance(items.ElementAt(j + 1).transform.position, transform.position))
                {
                    temp = items[j + 1];
                    items[j + 1] = items[j];
                    items[j] = temp;
                }
            }
        }
    }
}
