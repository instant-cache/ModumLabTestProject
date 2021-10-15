using System;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
    public float MaxFluidVolumeRepresentation = 1000;
    [SerializeField] [Range(0, 100)] public float MinFluidVolume = 80;
    [SerializeField] [Range(0, 100)] public float TargetGreenToBlueRatio = 70;
    [SerializeField] [Range(0, 100)] public float AllowedErrorMargin = 5;
    private PlayerController _playerController;
    private ValveController _greenValveController, _blueValveController;
    private FluidController _fluidController;
    private TabletUIController _menuController;
    private ReplayManager _replayManager;
    private string text;

    private float CurrentVolume;
    private float TargetVolume;
    private bool Ongoing = false;
    private bool IsWorking = true;
    public float timeStart;
    public float timeEnd;
    public float timeTotal;

    public class OnScenarioEventArgs : EventArgs
    {
        public float currentVolume, targetVolume, timeTotal, fluidRatio, targetFluidRatio, fluidRatioErrorMargin, greenFluidVolume, blueFluidVolume, valveActions;
        public bool success;
    }

    // Start is called before the first frame update
    void Start()
    {
        var gm = Camera.main.GetComponent<GameManager>();
        _playerController = gm.playerController;
        _greenValveController = gm.greenValveController;
        _blueValveController = gm.blueValveController;
        _fluidController = gm.fluidController;
        _menuController = gm.tabletController.tabletUIController;
        _replayManager = gm.replayManager;
        TargetVolume = GetTargetVolume();
        _fluidController.OnVolumeChanged += SetScenarioVariables;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Ongoing)
        {
            timeEnd += Time.fixedDeltaTime;
            IsWorking = GetValveWorkingState();
            if (CurrentVolume >= TargetVolume && !IsWorking)
            {
                EndScenario();
            }
        }
    }

    public event Action OnScenarioStart;
    public void StartScenario(bool IsReplay)
    {
        CurrentVolume = 0;
        if (!IsReplay)
        {
            _playerController.ToggleControls(true);
            _replayManager.StartRecording();
        }
        _menuController.ChangeState(TabletUIController.UIStates.InProgress);
        Ongoing = true;
        timeStart = Time.time;
        timeEnd = timeStart;
        OnScenarioStart();
    }

    public event Action<OnScenarioEventArgs> OnScenarioChanged;
    void SetScenarioVariables(float volume)
    {
        if (Ongoing)
        {
            CurrentVolume = volume * MaxFluidVolumeRepresentation;
            IsWorking = GetValveWorkingState();
            if (CurrentVolume >= TargetVolume && !IsWorking)
            {
                EndScenario();
            }
            else OnScenarioChanged(new OnScenarioEventArgs { currentVolume = CurrentVolume, blueFluidVolume = GetBlueFluidVolume(), 
                greenFluidVolume = GetGreenFluidVolume(), success = CurrentVolume >= TargetVolume });
        }
    }


    public event Action<OnScenarioEventArgs> OnScenarioEnd;
    public void EndScenario()
    {
        timeTotal = timeEnd - timeStart;
        _playerController.ToggleControls(false);
        _menuController.ChangeState(TabletUIController.UIStates.Finished);
        Ongoing = false;
        _replayManager.EndRecording();
        float fluidRatio = GetFluidRatio();
        bool success = fluidRatio >= TargetGreenToBlueRatio - AllowedErrorMargin && fluidRatio <= TargetGreenToBlueRatio + AllowedErrorMargin;
        OnScenarioEnd(new OnScenarioEventArgs() { blueFluidVolume = GetBlueFluidVolume(), greenFluidVolume = GetGreenFluidVolume(), 
            currentVolume = CurrentVolume, fluidRatio = fluidRatio, fluidRatioErrorMargin = AllowedErrorMargin, success = success, 
            targetFluidRatio = TargetGreenToBlueRatio, targetVolume = TargetVolume, timeTotal = timeTotal, valveActions = GetValveActions()});
    }

    public float GetTargetVolume()
    {
        return _fluidController.GetMaxFluidCapacity() * MaxFluidVolumeRepresentation * (MinFluidVolume / 100);
    }

    public float GetBlueFluidVolume()
    {
        return _fluidController.GetBlueFluidVolume() * MaxFluidVolumeRepresentation;
    }

    public float GetGreenFluidVolume()
    {
        return _fluidController.GetGreenFluidVolume() * MaxFluidVolumeRepresentation;
    }

    public float GetCurrentVolume()
    {
        return _fluidController.GetFluidVolume() * MaxFluidVolumeRepresentation;
    }

    public bool GetValveWorkingState()
    {
        if (_greenValveController.GetRotationRaw() > 0 || _blueValveController.GetRotationRaw() > 0)
        {
            return true;
        }
        return false;
    }

    public float GetFluidRatio()
    {
        return _fluidController.GetGreenToBlueFluidRatio() * 100;
    }

    public int GetValveActions()
    {
        return _blueValveController.GetValveActions() + _greenValveController.GetValveActions();
    }

    public float GetElapsedTime()
    {
        return timeTotal;
    }

    public void SetElapsedTime(float time)
    {
        timeTotal = time;
        timeStart = 0;
        timeEnd = timeTotal;
    }
}
