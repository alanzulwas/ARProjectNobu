using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewImagePrefab : MonoBehaviour
{
    public RawImage img;
    public int indexPrevButton;

    // Start is called before the first frame update
    public void PreviewImage(GameObject obj)
    {
		img.texture = RuntimePreviewGenerator.GenerateModelPreview(obj.transform);
	}

	public void setIndexPrev(int index)
	{
		indexPrevButton = index;
	}

	public void ObjectPreview()
	{
		ARToTapObject placem = ARToTapObject.Instance;
		placem.ClearObjectPreview();
		Debug.Log("Set preview index " + indexPrevButton);
		GameObject loadObject = Instantiate(placem._objectPrefab[indexPrevButton], placem.PlacementPose.position, placem.PlacementPose.rotation).gameObject;
		loadObject.transform.SetParent(placem.PreviewTf);
	}
}
