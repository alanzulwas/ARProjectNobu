using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewImagePrefab : MonoBehaviour
{
    public RawImage img;
    public int indexPrevButton;

	// Start is called before the first frame update
	private void Update()
	{
		//Debug.Log("Position " + transform.position);
	}

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
		placem.ObjectNumber = indexPrevButton;
		Debug.Log("Set preview index " + indexPrevButton);
		GameObject loadObject = Instantiate(placem._objectPrefab[indexPrevButton]).gameObject;
		loadObject.transform.position = new Vector3(0f,0f,2f);
		loadObject.transform.SetParent(placem.PreviewTf);
		placem._previewedObject = loadObject;
	}
}
