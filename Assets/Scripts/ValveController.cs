using System;
using System.Collections.Generic;
using UnityEngine;

public class ValveController : MonoBehaviour
{
    [Range(0.0f, 720.0f)]
    [SerializeField] public float MaxRotation = 720.0f;
    [SerializeField] private float CurrentRotation = 0;
    [SerializeField] private bool IsTurning = false;
    private static float MinRotation = 0f;
    private Plane HitboxPlane;
    private Vector3? OldPlaneHitCoord;
    private int ValveActions = 0;

    private PlayerController Player;

    // Start is called before the first frame update
    void Start()
    {
        HitboxPlane = new Plane(Vector3.up, transform.position);
        InitializeFromGameManager();
        Player = Camera.main.GetComponent<GameManager>().playerController;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public event Action<ValveController, float> OnValveRotated;
    void RotateValve()
    {
        Vector3? NewPlaneHitCoord = GetPlaneHitPoint();
        if (NewPlaneHitCoord == null)
        {
            Debug.LogWarning("Failed to find plane hit point!");
            return;
        }
        float resultAngle = Vector3.SignedAngle((Vector3)OldPlaneHitCoord - transform.position, (Vector3)NewPlaneHitCoord - transform.position, Vector3.up);
        if (CurrentRotation + resultAngle > MaxRotation)
            CurrentRotation = MaxRotation;
        else if (CurrentRotation + resultAngle < MinRotation)
            CurrentRotation = MinRotation;
        else CurrentRotation += resultAngle;
        this.transform.parent.eulerAngles = new Vector3(0, CurrentRotation);
        OldPlaneHitCoord = NewPlaneHitCoord;
        OnValveRotated?.Invoke(this, CurrentRotation);
    }

    private void OnMouseDown()
    {
        if (Player.ControlsEnabled)
        {
            var hitPoint = GetPlaneHitPoint();
            if (hitPoint != null)
            {
                OldPlaneHitCoord = hitPoint;
                ValveActions++;
            }
        }
    }

    private void OnMouseDrag()
    {
        if (Player.ControlsEnabled) RotateValve();
    }

    private void OnMouseUp()
    {

    }

    private Vector3? GetPlaneHitPoint()
    {
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (HitboxPlane.Raycast(_ray, out distance))
        {
            return _ray.GetPoint(distance);
        }
        return null;
    }

    //Normalizes rotation so that leftmost valve position always returns the same value
    public float GetRotationNormalized()
    {
        return 720 * CurrentRotation / MaxRotation;
    }

    public float GetRotationRaw()
    {
        return CurrentRotation;
    }

    void InitializeFromGameManager()
    {
        var gm = Camera.main.GetComponent<GameManager>();
        MaxRotation = gm.ValvesMaxRotation;
    }

    public int GetValveActions()
    {
        return ValveActions;
    }

    public ReplayEvent GetRotationReplayEvent()
    {
        return new ReplayEvent("rotation", CurrentRotation);
    }

    public void RotateByRecording(float _rotation)
    {
        CurrentRotation = _rotation;
        this.transform.parent.eulerAngles = new Vector3(0, CurrentRotation);
    }

    public void SetValveActions(int actions)
    {
        ValveActions = actions;
    }
}
