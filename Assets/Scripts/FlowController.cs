using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowController : MonoBehaviour
{
    public GameObject ConnectedValveObject;
    [SerializeField] private float DropletRate;
    [SerializeField] private float WaterStrength;
    [SerializeField] private bool NormalizeFlow;
    private ValveController valveController;
    private ParticleSystem WaterFlowParticleSystem;
    private float FlowDensity;
    // Start is called before the first frame update
    void Start()
    {
        InitializeFromGameManager();
        valveController = ConnectedValveObject.GetComponentInChildren<ValveController>();
        WaterFlowParticleSystem = GetComponent<ParticleSystem>();
        WaterFlowParticleSystem.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (NormalizeFlow)
            FlowDensity = valveController.GetRotationNormalized();
        else FlowDensity = valveController.GetRotationRaw();
        var emission = WaterFlowParticleSystem.emission;
        emission.rateOverTime = FlowDensity * DropletRate;
        var main = WaterFlowParticleSystem.main;
        main.startSpeed = FlowDensity * WaterStrength;
        if (!WaterFlowParticleSystem.isPlaying)
        {
            WaterFlowParticleSystem.Play();
        }
    }

    void InitializeFromGameManager()
    {
        var gm = Camera.main.GetComponent<GameManager>();
        DropletRate = gm.DropletRate;
        WaterStrength = gm.WaterStrength;
        NormalizeFlow = gm.NormalizeFlow;
    }

    public float GetFlowDensity()
    {
        return FlowDensity;
    }
}
