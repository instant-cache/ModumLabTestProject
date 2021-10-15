using System;
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
    public FlowController blueFlowController;
    public FlowController greenFlowController;
    [SerializeField] float ColourRatio = 0; //blue = 0, green = 1
    float volumeGreen = 0;

    void Start()
    {
        InitializeFluid();
        blueFlowController.OnFlowDensityChangedEvent += GetBlueFluidIntake;
        greenFlowController.OnFlowDensityChangedEvent += GetGreenFluidIntake;
    }

    void Update()
    {
        Fill();
    }

    public event Action<float> OnVolumeChanged;
    void Fill()
    {
        if (CurrentFluid < MaxFluidCapacity && (BlueFluidThroughput > 0 || GreenFluidThroughput > 0))
        {
            CurrentFluid += GetFluidIntakeCapacity(BlueFluidThroughput) + GetFluidIntakeCapacity(GreenFluidThroughput);
            if (CurrentFluid > MaxFluidCapacity) CurrentFluid = MaxFluidCapacity;
            this.transform.localScale = new Vector3(this.transform.localScale.x, CurrentFluid, this.transform.localScale.z);
            ChangeColour();
            OnVolumeChanged(CurrentFluid);
        }
    }
    public void Fill(float Volume)
    {
        CurrentFluid = Volume;
        this.transform.localScale = new Vector3(this.transform.localScale.x, CurrentFluid, this.transform.localScale.z);
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
        ChangeColour(ColourRatio);
    }
    public void ChangeColour(float Ratio)
    {
        ColourRatio = Ratio;
        this.GetComponent<Renderer>().material.color = Color.Lerp(Color.blue, Color.green, ColourRatio);
    }

    void GetBlueFluidIntake(float density)
    {
        BlueFluidThroughput = density;
    }
    void GetGreenFluidIntake(float density)
    {
        GreenFluidThroughput = density;
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

    public void InitializeFluid()
    {
        CurrentFluid = 0;
        ColourRatio = 0;
        volumeGreen = 0;
        this.transform.localScale = new Vector3(this.transform.localScale.x, 0, this.transform.localScale.z);
    }
}
