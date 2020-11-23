using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class GameManager : MonoBehaviour
{
    public GameObject ARPlacementInteractable;
    public List<GameObject> Interactables = new List<GameObject>();
    public List<GameObject> Targets = new List<GameObject>();
    public GameObject SelectedObject;

    public static GameManager Instance
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
        }
    }

    public async void StartGame()
    {
        List<GameObject> interactables = await Addressables.LoadAssetsAsync<GameObject>("Interactables", null).Task as List<GameObject>;
        for (int i = 0; i < 5; i++)
        {
            Vector3 position = VirtualAssistantManager.Instance.transform.TransformPoint(0, 0, 0.15f) + new Vector3((float)Math.Pow(-1, i) * 0.4f * (i / 2), 0f, 0f);
            GameObject interactable = Instantiate(interactables[i], position, interactables[i].transform.rotation);
            
            ARSelectionInteractable arSelectionInteractable = interactable.AddComponent<ARSelectionInteractable>();
            GameObject selection = Instantiate(Resources.Load<GameObject>("Selection"), interactable.transform);
            Bounds interactableBounds = interactable.GetComponent<Collider>().bounds;
            selection.transform.position = new Vector3(selection.transform.position.x, selection.transform.position.y - interactableBounds.extents.y, selection.transform.position.z);
            float radius = interactableBounds.extents.x > interactableBounds.extents.z ? interactableBounds.extents.x * 3 : interactableBounds.extents.z * 3;
            selection.transform.localScale = new Vector3(radius, selection.transform.localScale.y, radius);
            selection.SetActive(false);
            arSelectionInteractable.selectionVisualization = selection;
            
            ARTranslationInteractable arTranslationInteractable = interactable.AddComponent<ARTranslationInteractable>();
            arTranslationInteractable.objectGestureTranslationMode = GestureTransformationUtility.GestureTranslationMode.Any;
            arTranslationInteractable.maxTranslationDistance = 10;
            Interactables.Add(interactable);
        }

        List<GameObject> targets = await Addressables.LoadAssetsAsync<GameObject>("Targets", null).Task as List<GameObject>;
        for (int i = 0; i < targets.Count; i++)
        {
            Vector3 position = VirtualAssistantManager.Instance.transform.TransformPoint(0, 0, 0.15f) + new Vector3((float)Math.Pow(-1, i) * 0.4f * (i / 2), 0f, 0f);
            GameObject target = Instantiate(targets[i], position, targets[i].transform.rotation);
            target.AddComponent<CollisionManager>();
            Targets.Add(target);
        }

        Destroy(ARPlacementInteractable);
    }
}
