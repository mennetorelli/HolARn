using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class GameManager : MonoBehaviour
{
    public ARPlacementInteractable ARPlacementInteractable;
    public List<GameObject> InstantiatedInteractables = new List<GameObject>();
    public List<GameObject> InstantiatedTargets = new List<GameObject>();
    public GameObject SelectedObject;

    private List<GameObject> _interactables;
    private List<GameObject> _targets;

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

    async void Start()
    {
        _interactables = await Addressables.LoadAssetsAsync<GameObject>("Interactables", null).Task as List<GameObject>;
        _targets = await Addressables.LoadAssetsAsync<GameObject>("Targets", null).Task as List<GameObject>;

        ARPlacementInteractable.placementPrefab = await Addressables.LoadAssetAsync<GameObject>("VirtualAssistant").Task;
        ARPlacementInteractable.onObjectPlaced.AddListener(OnAssistantPlaced);
    }

    void OnAssistantPlaced(ARPlacementInteractable arPlacementInteractable, GameObject gameObject)
    {
        ARPlacementInteractable.onObjectPlaced.RemoveListener(OnAssistantPlaced);
        ARPlacementInteractable.onObjectPlaced.AddListener(OnTargetPlaced);
        SelectTargeToPlace();
    }

    void SelectTargeToPlace()
    {
        GameObject target = _targets[new System.Random().Next(0, _targets.Count - 1)];
        ARPlacementInteractable.placementPrefab = target;
        _targets.Remove(target);
    }

    void OnTargetPlaced(ARPlacementInteractable arPlacementInteractable, GameObject gameObject)
    {
        Debug.Log(gameObject);
        InstantiatedTargets.Add(gameObject);
        if (_targets.Count != 0)
        {
            SelectTargeToPlace();
        }
        else
        {
            ARPlacementInteractable.onObjectPlaced.RemoveListener(OnTargetPlaced);
            ARPlacementInteractable.onObjectPlaced.AddListener(OnInteractablePlaced);
            _interactables = _interactables.Where(i => InstantiatedTargets.Select(t => t.tag).Contains(i.tag)).ToList();
            SelectInteractableToPlace();
        }
    }

    void SelectInteractableToPlace()
    {
        GameObject interactable = _interactables[new System.Random().Next(0, _interactables.Count - 1)];
        ARPlacementInteractable.placementPrefab = interactable;
    }

    void OnInteractablePlaced(ARPlacementInteractable arPlacementInteractable, GameObject gameObject)
    {
        InstantiatedInteractables.Add(gameObject);
        if (InstantiatedInteractables.Count != 5)
        {
            SelectInteractableToPlace();
        }
        else
        {
            ARPlacementInteractable.onObjectPlaced.RemoveListener(OnInteractablePlaced);
            Destroy(ARPlacementInteractable);
            StartGame();
        }
    }

    async void StartGame()
    {
        foreach (GameObject interactable in InstantiatedInteractables)
        {
            ARSelectionInteractable arSelectionInteractable = interactable.AddComponent<ARSelectionInteractable>();
            GameObject selection = await Addressables.InstantiateAsync("Selection", interactable.transform).Task;
            Bounds interactableBounds = interactable.GetComponent<Collider>().bounds;
            selection.transform.position = new Vector3(selection.transform.position.x, selection.transform.position.y - interactableBounds.extents.y, selection.transform.position.z);
            float radius = interactableBounds.extents.x > interactableBounds.extents.z ? interactableBounds.extents.x * 3 : interactableBounds.extents.z * 3;
            selection.transform.localScale = new Vector3(radius, selection.transform.localScale.y, radius);
            selection.SetActive(false);
            arSelectionInteractable.selectionVisualization = selection;

            ARTranslationInteractable arTranslationInteractable = interactable.AddComponent<ARTranslationInteractable>();
            arTranslationInteractable.objectGestureTranslationMode = GestureTransformationUtility.GestureTranslationMode.Horizontal;
        }

        VirtualAssistantManager.Instance.Play();
    }
}
