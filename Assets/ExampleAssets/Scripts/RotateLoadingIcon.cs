using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoadingIcon : MonoBehaviour
{
	RectTransform rectTransform;
    float rotateZ;
	// Start is called before the first frame update



	void Start()
    {
		rectTransform = GetComponent<RectTransform>();
        rotateZ = 0f;
	}

    // Update is called once per frame
    void Update()
    {
        if (gameObject.active)
        {
			rectTransform.rotation = Quaternion.EulerAngles(0f, 0f, rotateZ);
			rotateZ -= 0.01f;
		}
    }
}
