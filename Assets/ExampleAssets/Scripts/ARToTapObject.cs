using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.UI;

public class ARToTapObject : MonoBehaviour
{
	public GameObject placementIndicator;
    public List<GameObject> _objectPrefab;
	public GameObject _previewedObject;

	[SerializeField] private Transform _objPreviewTransform;
    [SerializeField] private Transform _objPreviewHoldTransform;

    [Header("Button UI")]
    [SerializeField] private GameObject _panelButton;
    [SerializeField] private GameObject _objPrevBtnPrefab;
    [SerializeField] private GameObject _objAddBtnPrefab;
    public ARRaycastManager arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid;
    private int objectNumber = 0;

	void Start()
    {
        objectNumber = 0;
        FillPreviewObject();
    }

    public Pose PlacementPose { get => placementPose; set => placementPose = value; }
	public Transform PreviewTf { get => _objPreviewTransform; set => _objPreviewTransform = value; }
	public Transform PreviewTfHold { get => _objPreviewHoldTransform; set => _objPreviewHoldTransform = value; }
    public int ObjectNumber { get => objectNumber; set => objectNumber = value; }
	
	// Update is called once per frame
	void Update()
    {
        UpdatePreviewObject();
        //UpdatePlacementPose();
        //UpdatePlacementIndicator();
    }

    public void UpdatePreviewObject()
    {
        if (_panelButton.transform.childCount != _objectPrefab.Count)
        {
            foreach (Transform obj in _panelButton.transform)
            {
                Destroy(obj.gameObject);
            }
            FillPreviewObject();
		}
    }

    void FillPreviewObject()
    {
		for (int i = 0; i < _objectPrefab.Count; i++)
		{
			GameObject prevButton = Instantiate(_objPrevBtnPrefab, _panelButton.transform).gameObject;

			prevButton.name = _objectPrefab[i].name;

			prevButton.GetComponent<PreviewImagePrefab>().setIndexPrev(i);
			prevButton.GetComponent<Button>().onClick.RemoveAllListeners();
			prevButton.GetComponent<Button>().onClick.AddListener(() => { prevButton.GetComponent<PreviewImagePrefab>().ObjectPreview(); });

			PreviewImagePrefab comp = prevButton.GetComponent<PreviewImagePrefab>();

			comp.PreviewImage(_objectPrefab[i]);
		}
	}

    public void PlaceObject()
    {
		if (_objPreviewHoldTransform.childCount == 0)
        {
            return;
        }
        _previewedObject.transform.parent = null;
        RemoveAllComponent(_previewedObject);
        _previewedObject = null;
    }

    void RemoveAllComponent(GameObject loadObject)
    {
        foreach (var comp in loadObject.GetComponents<Component>())
        {
            if (!(comp is Transform) && !(comp is MeshFilter) && !(comp is MeshRenderer))
            {
                DestroyImmediate(comp);
            }
        }
    }

    public void ClearObjectPreview()
    {
        objectNumber = _objectPrefab.Count + 1;
        for(var i = _objPreviewHoldTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(_objPreviewHoldTransform.GetChild(i).gameObject);
        }
    }

	#region ARFoundation
	private void UpdatePlacementIndicator()
	{
		if (placementPoseIsValid)
		{
			placementIndicator.SetActive(true);
			placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
		}
		else
		{
			placementIndicator.SetActive(false);
		}
	}

	public void UpdatePlacementPose()
	{
		var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
		var hits = new List<ARRaycastHit>();
		arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

		placementPoseIsValid = hits.Count > 0;
		if (placementPoseIsValid)
		{
			placementPose = hits[0].pose;

			var cameraForward = Camera.current.transform.forward;
			var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
			placementPose.rotation = Quaternion.LookRotation(cameraBearing);
		}
	}
	#endregion

	private static ARToTapObject _instance;

    public static ARToTapObject Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ARToTapObject>();
            }
            return _instance;
        }
    }


}
