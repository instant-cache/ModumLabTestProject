using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
    public float MaxFluidVolumeRepresentation = 1000;
    [SerializeField] [Range(0, 100)] public float MinFluidVolume = 80;
    [SerializeField] [Range(0, 100)] public float TargetGreenToBlueRatio = 70;
    [SerializeField] [Range(0, 100)] public float AllowedErrorMargin = 5;
    public PlayerController _playerController;
    public ValveController _greenValveController, _blueValveController;
    public FluidController _fluidController;
    public TabletUIController _menuController;
    public ReplayManager _replayManager;

    private float CurrentVolume;
    private float TargetVolume;
    private bool Ongoing = false;
    private bool IsWorking = true;
    public float timeStart;
    public float timeEnd;
    public float timeTotal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Ongoing)
        {
            CurrentVolume = GetCurrentVolume();
            TargetVolume = GetTargetVolume();
            IsWorking = GetValveWorkingState();
            timeEnd += Time.deltaTime;
            if (CurrentVolume >= TargetVolume && !IsWorking)
            {
                EndScenario();
            }
        }
    }

    public void StartScenario()
    {
        _playerController.ToggleControls(true);
        _menuController.ChangeState(TabletUIController.UIStates.InProgress);
        Ongoing = true;
        timeStart = Time.time;
        timeEnd = timeStart;
        _replayManager.StartRecording();
    }

    public void EndScenario()
    {
        timeTotal = timeEnd - timeStart;
        _playerController.ToggleControls(false);
        _menuController.ChangeState(TabletUIController.UIStates.Finished);
        Ongoing = false;
        _replayManager.EndRecording();
    }

    public float GetTargetVolume()
    {
        return _fluidController.GetMaxFluidCapacity() * MaxFluidVolumeRepresentation * (MinFluidVolume / 100);
    }

    public float GetBlueFluidVolumeRepresented()
    {
        return _fluidController.GetBlueFluidVolume() * MaxFluidVolumeRepresentation;
    }

    public float GetGreenFluidVolumeRepresented()
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
