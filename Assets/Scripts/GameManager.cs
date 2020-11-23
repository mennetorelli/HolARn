using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
            Vector3 position = VirtualAssistantManager.Instance.transform.TransformPoint(0, 0, 0.2f) + new Vector3((float)Math.Pow(-1, i) * 0.4f * (i / 2), 0f, 0f);
            Interactables.Add(Instantiate(interactables[i], position, interactables[i].transform.rotation));
        }

        List<GameObject> targets = await Addressables.LoadAssetsAsync<GameObject>("Targets", null).Task as List<GameObject>;
        for (int i = 0; i < targets.Count; i++)
        {
            Vector3 position = VirtualAssistantManager.Instance.transform.TransformPoint(0, 0, -0.2f) + new Vector3((float)Math.Pow(-1, i) * 0.4f * (i / 2), 0f, 0f);
            Interactables.Add(Instantiate(targets[i], position, targets[i].transform.rotation));
        }

        Destroy(ARPlacementInteractable);
    }
}
