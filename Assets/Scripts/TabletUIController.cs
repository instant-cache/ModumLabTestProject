using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabletUIController : MonoBehaviour
{
    public enum UIStates { Start, InProgress, Finished }
    public UIStates InitialUIState = UIStates.Start;
    private UIStates CurrentUIState;
    public bool IsPortrait = true;
    private Scenario _scenario;

    public GameObject MainMenu;
    public GameObject InProgressStatusMenuPortrait;
    public GameObject InProgressStatusMenuLandscape;
    public GameObject FinishedStatusMenuPortrait;
    public GameObject FinishedStatusMenuLandscape;
    private GameObject InProgressStatusMenu;
    private GameObject FinishedStatusMenu;

    private float TargetVolume;
    private float CurrentVolume;
    private float GreenFluidVolume;
    private float BlueFluidVolume;
    private float FluidRatio;
    private float TargetFluidRatio;
    private float FluidRatioErrorMargin;
    private float ValveActions;
    private float ElapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(InitialUIState);
        InProgressStatusMenu = InProgressStatusMenuPortrait;
        FinishedStatusMenu = FinishedStatusMenuPortrait;
        _scenario = Camera.main.GetComponent<GameManager>().scenario;
    }

    private void Update()
    {
        if (CurrentUIState == UIStates.InProgress)
        {
            FetchInProgressData();
            SetInProgressData();
        }
    }

    public void ChangeState(UIStates _state)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        switch (_state) 
        {
            case UIStates.Start : 
                MainMenu.SetActive(true);
                break;
            case UIStates.InProgress :
                InProgressStatusMenu.SetActive(true);
                break;
            case UIStates.Finished :
                FinishedStatusMenu.SetActive(true);
                FetchFinishedData();
                SetFinishedData();
                break;
        }
        CurrentUIState = _state;
    }

    public void ChangeOrientation()
    {
        IsPortrait = !IsPortrait;
        if (IsPortrait)
        {
            InProgressStatusMenu = InProgressStatusMenuPortrait;
            FinishedStatusMenu = FinishedStatusMenuPortrait;
        }
        else
        {
            InProgressStatusMenu = InProgressStatusMenuLandscape;
            FinishedStatusMenu = FinishedStatusMenuLandscape;
        }
        ChangeState(CurrentUIState);
    }

    private void FetchInProgressData()
    {
        TargetVolume = _scenario.GetTargetVolume();
        CurrentVolume = _scenario.GetCurrentVolume();
        GreenFluidVolume = _scenario.GetGreenFluidVolumeRepresented();
        BlueFluidVolume = _scenario.GetBlueFluidVolumeRepresented();
    }

    private void SetInProgressData()
    {
        string colour;
        if (CurrentVolume < TargetVolume)
            colour = "red";
        else colour = "green";
        InProgressStatusMenu.GetComponentInChildren<Text>().text =
            $"Overall volume: <color={colour}>{CurrentVolume:0.##}L</color>\n" +
            $"Green fluid volume: {GreenFluidVolume:0.##}L\n" +
            $"Blue fluid volume: {BlueFluidVolume:0.##}L";
    }

    private void FetchFinishedData()
    {
        FetchInProgressData();
        FluidRatio = _scenario.GetFluidRatio();
        TargetFluidRatio = _scenario.TargetGreenToBlueRatio;
        FluidRatioErrorMargin = _scenario.AllowedErrorMargin;
        ValveActions = _scenario.GetValveActions();
        ElapsedTime = _scenario.GetElapsedTime();
    }

    private void SetFinishedData()
    {
        string colour = "red";
        string result = "FAILED!";
        if (FluidRatio >= TargetFluidRatio - FluidRatioErrorMargin && FluidRatio <= TargetFluidRatio + FluidRatioErrorMargin)
        {
            colour = "lime";
            result = "COMPLETE!";
        }
        FinishedStatusMenu.transform.Find("CompletionText").GetComponent<Text>().text = $"<color={colour}>{result}</color>";
        FinishedStatusMenu.transform.Find("StatusText").GetComponent<Text>().text =
            $"End volume: {CurrentVolume:0.#}L/{TargetVolume:0.#}L\n" +
            $"Green fluid: {GreenFluidVolume:0.##}L\n" +
            $"Blue fluid: {BlueFluidVolume:0.##}L\n" +
            $"End fluid ratio: {FluidRatio:0.##}%/{TargetFluidRatio:0.##}±{FluidRatioErrorMargin:0.##}%\n" +
            $"Valve actions: {ValveActions}\n" +
            $"Time taken: {Mathf.FloorToInt(ElapsedTime/60)}:{(int)ElapsedTime%60}";
    }
}
