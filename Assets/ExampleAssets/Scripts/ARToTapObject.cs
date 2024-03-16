using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using IngameDebugConsole;
using System.IO;

public class ARToTapObject : MonoBehaviour
{
	public GameObject placementIndicator;
    public List<GameObject> _objectPrefab;
	public GameObject _previewedObject;
    public GameObject _temporaryGameObject;
    public Material _materialDefault;
    public TextMeshProUGUI _countingCache;
    public int angkaCacheSaved;
    public List<string> DataPersistentStringValue; 

	[SerializeField] private Transform _objPreviewTransform;
    [SerializeField] private Transform _objPreviewHoldTransform;

    [Header("UI")]
    [SerializeField] private GameObject _panelButton;
    [SerializeField] private GameObject _objPrevBtnPrefab;
    [SerializeField] private GameObject _objAddBtnPrefab;
    public ARRaycastManager arOrigin;

	[Header("AssetBundle")]
	string assetBundleLink = "";

	bool clearCache = false;
	private GameObject placementARObject = null;
	private AssetBundle assetBundle = null;

	public Natipfilpick _importerLoader;
    private Pose placementPose;
    private bool placementPoseIsValid;
    private int objectNumber = 0;

	private void Awake()
	{
		//if (!Directory.Exists(Application.persistentDataPath + "/Materials"))
  //      {
  //          Directory.CreateDirectory(Application.persistentDataPath + "/Materials");
  //              //CreateFolder(Application.persistentDataPath, "Materials");
		//}
		//if (!File.Exists(Application.persistentDataPath + "/Materials/Testing.mat"))
  //      {
  //          File.Copy(Application.dataPath+"/Material/Testing.mat", Application.persistentDataPath + "/Materials/Testing.mat");
  //      }	
	}

	void Start()
    {
        objectNumber = 0;

        _importerLoader.StatusObjectCountText.SetActive(true);
        _importerLoader.LoadingScreen.SetActive(false);

		//      Debug.Log(Application.dataPath);
		Debug.Log(Application.persistentDataPath);
		//Debug.Log(Application.temporaryCachePath);
		//Debug.Log(Application.streamingAssetsPath);


        UpdateObjectPrefab();
	}

    public Pose PlacementPose { get => placementPose; set => placementPose = value; }
	public Transform PreviewTf { get => _objPreviewTransform; set => _objPreviewTransform = value; }
	public Transform PreviewTfHold { get => _objPreviewHoldTransform; set => _objPreviewHoldTransform = value; }
    public int ObjectNumber { get => objectNumber; set => objectNumber = value; }
	
	// Update is called once per frame
	void Update()
    {
        //UpdateObjectPrefab();
        //UpdatePreviewObject();
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        //UpdateStatusObject();

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
        DataPersistentStringValue = null;
        DataPersistentStringValue = new List<string>();

        string persistent = Application.persistentDataPath;
        //string persistent = "asdkajshd";

        var fileNames = Directory.GetFiles(persistent + "/Object3Dimport" + "/");
		foreach (var fileName in fileNames)
		{
			//PrefabAsset.Add(fileName);\
			DataPersistentStringValue.Add(fileName);
		}

		angkaCacheSaved = 0;
        foreach (GameObject obj in _objectPrefab)
        {
            Debug.Log("Hapus Object");
            Destroy(obj);
        }

        _objectPrefab = null;
        _objectPrefab = new List<GameObject>();



        for(int i = 0; i < DataPersistentStringValue.Count; i++)
        {

			try
            {
				//GameObject tempObject = new OBJLoader().Load(DataPersistentStringValue[i],Application.persistentDataPath+"/Materials/Testing.mat");
				//GameObject tempObject = new OBJLoader().Load(DataPersistentStringValue[i], @"Assets/Material/Testing.mat");
				InitiateLoadModel();
				Debug.Log("Update Object Prefab");
			}
            catch (Exception e) 
            {
                Debug.LogError(e);
                Debug.LogError("Ada Data yang hilang dari Direktori ini : " + persistent + "/Object3Dimport"
					+ "\nTolong Hapus Directory tersebut dan Install ulang Aplikasi ini");

			}
            angkaCacheSaved++;
        }

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

	private void InitiateLoadModel()
	{
		Caching.compressionEnabled = false;

		if (clearCache)
		{
			Caching.ClearCache();
		}
		StartCoroutine(DownloadAndLoad());
	}

	private IEnumerator DownloadAndLoad()
	{
		while (!Caching.ready)
		{
			yield return null;
		}
		//yield return GetBundle();

		var assetBundler = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "Android/bigplane"));

		if (!assetBundler)
		{
			Debug.Log("Bundle Failed to Load");
			yield break;
		}

		Debug.Log(assetBundler);
		GameObject tempObject = assetBundler.LoadAsset<GameObject>("bigplane");
		tempObject.transform.SetParent(_temporaryGameObject.transform);
		_objectPrefab.Add(tempObject);
	}

	private IEnumerator GetBundle()
	{
		WWW request = WWW.LoadFromCacheOrDownload("https://drive.google.com/file/d/1myaNjw_d8osiJOhjCjWRPnn1Uj_aeDJY/view?usp=sharing", 0);
		while (!request.isDone)
		{
			yield return null;
		}
		if (request.error == null)
		{
			//Debug.Log();
			assetBundle = request.assetBundle;
			Debug.Log("Success");
		}
		else
		{
			Debug.Log("Error" + request.error);
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
