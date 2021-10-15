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
    private float FlowDensity = 0;

    // Start is called before the first frame update
    void Start()
    {
        InitializeFromGameManager();
        valveController = ConnectedValveObject.GetComponentInChildren<ValveController>();
        WaterFlowParticleSystem = GetComponent<ParticleSystem>();
        WaterFlowParticleSystem.Play();
        var emission = WaterFlowParticleSystem.emission;
        emission.rateOverTime = 0;
        var main = WaterFlowParticleSystem.main;
        main.startSpeed = 0;
        valveController.OnValveRotated += SetFlowDensity;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetFlowDensity(ValveController sender, float Rotation)
    {
        FlowDensity = Rotation;
        if (NormalizeFlow)
            FlowDensity = 720 * FlowDensity / sender.MaxRotation;
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
