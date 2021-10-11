using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabletDragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector3 RememberedMousePosition;
    private Vector3 MouseDragOffset;
    private float CameraDistance;
    private Plane HitboxPlane;
    private bool IsMoving = false;

    public void OnDrag(PointerEventData eventData)
    {
        if (IsMoving)
        {
            var hitPoint = GetPlaneHitPoint();
            if (hitPoint != null)
            {
                var NewMousePosition = GetPlaneHitPoint();
                var Translation = NewMousePosition - MouseDragOffset;
                transform.parent.parent.position = Translation;
            }
            HitboxPlane = new Plane(Camera.main.transform.forward, Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)).GetPoint(CameraDistance));
            RememberedMousePosition = GetPlaneHitPoint();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        HitboxPlane = new Plane(Camera.main.transform.forward, transform.position);
        Debug.Log("Trying to create plane");
        var hitPoint = GetPlaneHitPoint();
        if (hitPoint != null)
        {
            IsMoving = true;
            RememberedMousePosition = GetPlaneHitPoint();
            MouseDragOffset = RememberedMousePosition - this.transform.parent.parent.position;
            HitboxPlane.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out CameraDistance);
        }
        else Debug.LogError("Failed to find plane hit point!");
    }

    private Vector3 GetPlaneHitPoint()
    {
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        HitboxPlane.Raycast(_ray, out distance);
        return _ray.GetPoint(distance);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsMoving = false;
    }

    public void OnDrawGizmos()
    {
        //Gizmos.DrawCube(RememberedMousePosition, new Vector3(0.1f, 0.1f, 0.1f));
    }
}
