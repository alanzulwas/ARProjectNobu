using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGesture : MonoBehaviour
{
	private void OnMouseDrag()
	{
		Vector3 MousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z + transform.position.z);

		Vector3 objPosition = Camera.main.ScreenToWorldPoint(MousePosition);
		
		transform.position = objPosition;
	}
}
