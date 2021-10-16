using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool ControlsEnabled = false;
    public bool IsReplaying = false;

    const int RightMouseButton = 1;
    [SerializeField] private float Speed;
    [SerializeField] private float MouseSensitivityX;
    [SerializeField] private float MouseSensitivityY;
    [SerializeField] private bool InvertMouseX;
    [SerializeField] private bool InvertMouseY;
    [SerializeField] [Range(-179f, 179f)] private float MaxCameraAngle;
    [SerializeField] [Range(-179f, 179f)] private float MinCameraAngle;
    public TabletController iPadController;

    Camera MainCamera;
    Rigidbody rb;
    Vector2 CurrentCameraAngle;
    Vector2 MovementVectorFlat = Vector2.zero;
    Vector2 LookVector = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main;
        var gm = MainCamera.GetComponent<GameManager>();
        MaxCameraAngle = gm.MaxCameraAngle;
        MinCameraAngle = gm.MinCameraAngle;
        rb = GetComponent<Rigidbody>();
        CurrentCameraAngle = new Vector2(MainCamera.transform.eulerAngles.y, MainCamera.transform.localEulerAngles.x);
        InitializeFromGameManager();
    }

    // Update is called once per frame
    void Update()
    {
        if (ControlsEnabled)
        {
            if (IsMouseLook())
            {
                LookVector = GetMouseMovement();
                MouseLook();
            }
            //else MouseControl();
            MovementVectorFlat = GetMovement();
        }
        TabletControls();
    }

    void FixedUpdate()
    {
        if (MinCameraAngle > MaxCameraAngle) MaxCameraAngle = MinCameraAngle;
        SetMovement();
        //if (IsMouseLook()) MouseLook();
    }

    Vector2 GetMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return new Vector2(vertical, horizontal).normalized;
    }

    void SetMovement()
    {
        if (rb == null)
        {
            Debug.LogError("Failed to get rb!");
            return;
        }
        Vector3 Movement = MainCamera.transform.right * MovementVectorFlat.y + MainCamera.transform.forward * MovementVectorFlat.x;
        Movement.y = 0;
        rb.MovePosition(transform.position + Movement * Time.fixedDeltaTime * Speed);
    }

    bool IsMouseLook()
    {
        return Input.GetMouseButton(RightMouseButton);
    }

    void MouseLook()
    {
        if (ControlsEnabled)
        {
            //Debug.Log($"Look vector: {LookVector}");
            if (InvertMouseX) LookVector.x = -LookVector.x;
            if (InvertMouseY) LookVector.y = -LookVector.y;
            CurrentCameraAngle.x += LookVector.x * Time.fixedDeltaTime * MouseSensitivityX;
            this.transform.eulerAngles = new Vector3(0, CurrentCameraAngle.x, 0);
            if (CurrentCameraAngle.y + LookVector.y <= MaxCameraAngle && CurrentCameraAngle.y + LookVector.y >= MinCameraAngle)
            {
                CurrentCameraAngle.y += LookVector.y * Time.fixedDeltaTime * MouseSensitivityY;
                MainCamera.transform.localEulerAngles = new Vector3(CurrentCameraAngle.y, 0, 0);
            }
        }
    }

    public static Vector2 GetMouseMovement()
    {
        float horizontal = Input.GetAxisRaw("Mouse X");
        float vertical = Input.GetAxisRaw("Mouse Y");
        return new Vector2(horizontal, vertical);
    }

    void InitializeFromGameManager()
    {
        var gm = MainCamera.GetComponent<GameManager>();
        Speed = gm.PlayerSpeed;
        MouseSensitivityX = gm.MouseSensitivityX;
        MouseSensitivityY = gm.MouseSensitivityY;
        InvertMouseX = gm.InvertMouseX;
        InvertMouseY = gm.InvertMouseY;
        MaxCameraAngle = gm.MaxCameraAngle;
        MinCameraAngle = gm.MinCameraAngle;
    }

    public void ToggleControls()
    {
        Debug.Log($"Toggled player controls. Now {ControlsEnabled}");
        ControlsEnabled = !ControlsEnabled;
    }
    public void ToggleControls(bool state)
    {
        Debug.Log($"Toggled player controls. Now {state}");
        ControlsEnabled = state;
    }

    public event Func<bool>  OnTabletRotated;
    public event Action OnTabletHidden;
    void TabletControls()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            bool ok = OnTabletRotated();
            if (!ok)
            {
                Debug.LogError("Can't turn tablet while it's hidden");
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OnTabletHidden();
        }
    }

    public Vector3 GetPositionReplayEvent()
    {
        return transform.position;
    }

    public void MoveByRecording(Vector3 _movement)
    {
        this.transform.position = _movement;
    }

    public void RotateCameraByRecording(Vector3 _camera)
    {
        Camera.main.transform.rotation = Quaternion.Euler(_camera);
    }
}
