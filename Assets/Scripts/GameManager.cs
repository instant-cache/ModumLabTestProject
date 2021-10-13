using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Valve Controller settings")]
    [Tooltip("Sets valves' maximum rotation in Euler degrees")]
    [SerializeField] [Range(0f, 720f)] public float ValvesMaxRotation = 720f;

    [Header("Player Controller settings")]
    [Tooltip("Sets player movement speed")]
    [SerializeField] public float PlayerSpeed = 1;
    [Tooltip("Sets horizontal mouse sensitivity")]
    [SerializeField] public float MouseSensitivityX = 1;
    [Tooltip("Sets vertical mouse sensitivity")]
    [SerializeField] public float MouseSensitivityY = 1;
    [Tooltip("Inverts horizontal mouse movement")]
    [SerializeField] public bool InvertMouseX = false;
    [Tooltip("Inverts vertical mouse movement")]
    [SerializeField] public bool InvertMouseY = false;
    [Tooltip("Sets maximum angle the camera can turn upwards")]
    [SerializeField] [Range(-179f, 179f)] public float MaxCameraAngle = 179f;
    [Tooltip("Sets maximum angle the camera can turn downwards")]
    [SerializeField] [Range(-179f, 179f)] public float MinCameraAngle = -179f;

    [Header("Particle Controller settings")]
    [Tooltip("Sets droplet multiplication rate (higher = more droplets appear per degree of valve turn)")]
    [SerializeField] public float DropletRate = 1;
    [Tooltip("Sets droplet stream strength multiplier (higher = droplets go faster per degree of valve turn")]
    [SerializeField] public float WaterStrength = 1;
    [Tooltip("Normalizes droplet intensity against max valve rotation")]
    [SerializeField] public bool NormalizeFlow = true;

    [Header("Controller references")]
    public ReplayManager replayManager;
    public PlayerController playerController;
    public ValveController greenValveController, blueValveController;
    public TabletController tabletController;
    public FluidController fluidController;
    public Scenario scenario;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
