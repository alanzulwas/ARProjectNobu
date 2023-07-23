using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class ARToTapObject : MonoBehaviour
{
    public GameObject placementIndicator;
    public List<GameObject> _objectPrefab;

    [SerializeField] private Transform _objPreviewTransform;
    public ARRaycastManager arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid;
    private int objectNumber = 0;

    void Start()
    {
        objectNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    public void PlaceObject()
    {
        GameObject selectedObject = _objectPrefab[objectNumber];
        GameObject loadObject = Instantiate(selectedObject, placementPose.position, placementPose.rotation).gameObject;
        loadObject.transform.SetParent(_objPreviewTransform);
        loadObject.transform.parent = null;
    }

    public void ObjectPreview(int preview)
    {
        objectNumber = preview;
        ClearObjectPreview();
        GameObject loadObject = Instantiate(_objectPrefab[preview], placementPose.position, placementPose.rotation).gameObject;
        loadObject.transform.SetParent(_objPreviewTransform);
    }

    public void ClearObjectPreview()
    {
        objectNumber = _objectPrefab.Count + 1;
        for(var i = _objPreviewTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(_objPreviewTransform.GetChild(i).gameObject);
        }
    }

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
