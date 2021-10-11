using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidController : MonoBehaviour
{
    public float MaxFluidCapacity = 0.8f;
    public float ThroughputMultiplier = 0.01f;
    [SerializeField] float CurrentFluid = 0;
    Vector3 scale;
    float BlueFluidThroughput = 0;
    float GreenFluidThroughput = 0;
    public GameObject BlueFluid;
    public GameObject GreenFluid;
    [SerializeField] float ColourRatio = 0; //blue = 0, green = 1
    float volumeGreen = 0;

    void Start()
    {

    }

    void Update()
    {
        GetFluidIntake();
        if (CurrentFluid < MaxFluidCapacity && (BlueFluidThroughput > 0 || GreenFluidThroughput > 0))
        {
            ChangeColour();
            Fill();
        }
    }

    void Fill()
    {
        CurrentFluid += GetFluidIntakeCapacity(BlueFluidThroughput) + GetFluidIntakeCapacity(GreenFluidThroughput);
        if (CurrentFluid > MaxFluidCapacity) CurrentFluid = MaxFluidCapacity;
        this.transform.localScale = new Vector3(this.transform.localScale.x, CurrentFluid, this.transform.localScale.z);
    }

    void GetFluidIntake()
    {
        BlueFluidThroughput = BlueFluid.GetComponent<FlowController>().GetFlowDensity();
        GreenFluidThroughput = GreenFluid.GetComponent<FlowController>().GetFlowDensity();
    }

    void ChangeColour()
    {
        if (CurrentFluid <= 0)
        {
            if (BlueFluidThroughput > 0) ColourRatio = 0;
            else ColourRatio = 1;
            return;
        }
        var BlueIntake = GetFluidIntakeCapacity(BlueFluidThroughput);
        var GreenIntake = GetFluidIntakeCapacity(GreenFluidThroughput);
        var NewFluid = CurrentFluid + BlueIntake + GreenIntake;
        if (BlueIntake <= 0 && ColourRatio == 1)
        {
            volumeGreen = NewFluid;
        }
        else if (GreenIntake <= 0 && ColourRatio == 0)
        {
            volumeGreen = 0;
        }
        else
        {
            volumeGreen += GreenIntake;
            ColourRatio = volumeGreen / NewFluid;
        }
        this.GetComponent<Renderer>().material.color = Color.Lerp(Color.blue, Color.green, ColourRatio);
    }

    float GetFluidIntakeCapacity(float _throughput)
    {
        return _throughput * ThroughputMultiplier * Time.deltaTime;
    }

    public float GetFluidVolume()
    {
        return CurrentFluid;
    }

    public float GetGreenFluidVolume()
    {
        return CurrentFluid * ColourRatio;
    }

    public float GetBlueFluidVolume()
    {
        return CurrentFluid * (1 - ColourRatio);
    }

    public float GetGreenToBlueFluidRatio()
    {
        return ColourRatio;
    }

    public float GetMaxFluidCapacity()
    {
        return MaxFluidCapacity;
    }

    public void ResetFluid()
    {
        CurrentFluid = 0;
        ColourRatio = 0;
        volumeGreen = 0;
    }
}
