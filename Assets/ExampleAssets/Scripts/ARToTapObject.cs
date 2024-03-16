using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;

public class ARToTapObject : MonoBehaviour
{
	public GameObject placementIndicator;
    public List<GameObject> _objectPrefab;
	public GameObject _previewedObject;
    public GameObject _temporaryGameObject;
    public TextMeshProUGUI _countingCache;
    public int angkaCacheSaved;

	[SerializeField] private Transform _objPreviewTransform;
    //[SerializeField] private Transform _objPreviewHoldTransform;

    [Header("UI")]
    [SerializeField] private GameObject _panelButton;
    [SerializeField] private GameObject _objPrevBtnPrefab;
    [SerializeField] private GameObject _objAddBtnPrefab;
    public ARRaycastManager arOrigin;

    [SerializeField] private GameObject _uiLoading;
    [SerializeField] private GameObject _placeObjectBtn;
    [SerializeField] private GameObject _panelObject;

    private Pose placementPose;
    private bool placementPoseIsValid;

	void Start()
    {
        UpdateObjectPrefab();
	}

    public Pose PlacementPose { get => placementPose; set => placementPose = value; }
	public Transform PreviewTf { get => _objPreviewTransform; set => _objPreviewTransform = value; }
	//public Transform PreviewTfHold { get => _objPreviewHoldTransform; set => _objPreviewHoldTransform = value; }
	
	// Update is called once per frame
	void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
	}

    public void UpdatePreviewObject()
    {
        _temporaryGameObject.SetActive(true);
        foreach (Transform obj in _panelButton.transform)
        {
            Destroy(obj.gameObject);
        }
        FillPreviewObject();
        _temporaryGameObject.SetActive(false);
	}

    void UpdateStatusObject()
    {
        angkaCacheSaved = _objectPrefab.Count;
		_countingCache.text = "Object yang bisa disimpan :\n\n" + angkaCacheSaved + " / 10";
	}

    public void UpdateObjectPrefab()
    {
        UpdatePreviewObject();
        UpdateStatusObject();

	}

    void FillPreviewObject()
    {
		for (int i = 0; i < _objectPrefab.Count; i++)
		{
			GameObject prevButton = Instantiate(_objPrevBtnPrefab, _panelButton.transform).gameObject;

			prevButton.name = _objectPrefab[i].name;
            prevButton.transform.Find("ObjectName").gameObject.GetComponent<TextMeshProUGUI>().text = _objectPrefab[i].name;

			prevButton.GetComponent<PreviewImagePrefab>().setIndexPrev(i);
			prevButton.GetComponent<Button>().onClick.RemoveAllListeners();
			prevButton.GetComponent<Button>().onClick.AddListener(() => { prevButton.GetComponent<PreviewImagePrefab>().ObjectPreview(); });

			PreviewImagePrefab comp = prevButton.GetComponent<PreviewImagePrefab>();

			comp.PreviewImage(_objectPrefab[i]);
		}
	}

    public void PlaceObject()
    {
		if (/*_objPreviewHoldTransform.childCount == 0 &&*/ _objPreviewTransform.childCount == 0)
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
        //objectNumber = _objectPrefab.Count + 1;
        //for(var i = _objPreviewHoldTransform.childCount - 1; i >= 0; i--)
        //{
        //    Destroy(_objPreviewHoldTransform.GetChild(i).gameObject);
        //}
        for (var i = _objPreviewTransform.childCount - 1; i >=0; i--)
        {
			Destroy(_objPreviewTransform.GetChild(i).gameObject);
		}
    }

	#region ARFoundation
	private void UpdatePlacementIndicator()
	{
		if (placementPoseIsValid)
		{
			placementIndicator.SetActive(true);
			_uiLoading.SetActive(false);
			_placeObjectBtn.SetActive(true);
			_panelObject.SetActive(true);
			placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
		}
		else
		{
			placementIndicator.SetActive(false);
            _uiLoading.SetActive(true);
            _placeObjectBtn.SetActive(false);
            _panelObject.SetActive(false);
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
