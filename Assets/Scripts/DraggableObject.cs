using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 curScreenPoint;
    private Vector3 curPosition;
    private Vector3 offset;

    private void OnMouseDown()
    {
        if (Manager.creating)
        {
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }
    }

    private void OnMouseDrag()
    {
        if (Manager.creating)
        {
            curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            transform.position = curPosition;
        }
    }
}