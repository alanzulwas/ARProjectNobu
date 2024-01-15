using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewImagePrefab : MonoBehaviour
{
    public RawImage img;

    // Start is called before the first frame update
    public void PreviewImage(GameObject obj)
    {
		img.texture = RuntimePreviewGenerator.GenerateModelPreview(obj.transform);
	}
}
