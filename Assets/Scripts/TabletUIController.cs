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
    private GameObject CurrentMenu;

    private string StatusText;
    private string CompletionText;
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
        _scenario.OnScenarioStart += SetupInProgress;
        _scenario.OnScenarioChanged += SetInProgressData;
        _scenario.OnScenarioEnd += SetFinished;
    }

    private void Update()
    {

    }

    public void ChangeState(UIStates _state)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        switch (_state) 
        {
            case UIStates.Start : 
                CurrentMenu = MainMenu;
                break;
            case UIStates.InProgress :
                CurrentMenu = InProgressStatusMenu;
                break;
            case UIStates.Finished :
                CurrentMenu = FinishedStatusMenu;
                break;
        }
        CurrentMenu.SetActive(true);
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
        InProgressStatusMenu.transform.Find("StatusText").GetComponent<Text>().text = StatusText;
        FinishedStatusMenu.transform.Find("StatusText").GetComponent<Text>().text = StatusText;
        FinishedStatusMenu.transform.Find("CompletionText").GetComponent<Text>().text = CompletionText;
        ChangeState(CurrentUIState);
    }

    private void SetupInProgress()
    {
        ChangeState(UIStates.InProgress);
        SetInProgressData(new Scenario.OnScenarioEventArgs());
    }

    private void SetInProgressData(Scenario.OnScenarioEventArgs e)
    {
        string colour;
        if (e.success)
            colour = "green";
        else colour = "red";
        StatusText = 
            $"Overall volume: <color={colour}>{e.currentVolume:0.##}L</color>\n" +
            $"Green fluid volume: {e.greenFluidVolume:0.##}L\n" +
            $"Blue fluid volume: {e.blueFluidVolume:0.##}L";
        CurrentMenu.GetComponentInChildren<Text>().text = StatusText;
    }

    private void SetFinished(Scenario.OnScenarioEventArgs e)
    {
        ChangeState(UIStates.Finished);
        string colour;
        string result;
        if (e.success)
        {
            colour = "lime";
            result = "COMPLETE!";
        } else
        {
            colour = "red";
            result = "FAILED!";
        }
        CompletionText = CurrentMenu.transform.Find("CompletionText").GetComponent<Text>().text = $"<color={colour}>{result}</color>";
        StatusText =
            $"End volume: {e.currentVolume:0.#}L/{e.targetVolume:0.#}L\n" +
            $"Green fluid: {e.greenFluidVolume:0.##}L\n" +
            $"Blue fluid: {e.blueFluidVolume:0.##}L\n" +
            $"End fluid ratio: {e.fluidRatio:0.##}%/{e.targetFluidRatio:0.##}±{e.fluidRatioErrorMargin:0.##}%\n" +
            $"Valve actions: {e.valveActions}\n" +
            $"Time taken: {Mathf.FloorToInt(e.timeTotal/60)}:{(int)e.timeTotal%60}";
        CurrentMenu.transform.Find("StatusText").GetComponent<Text>().text = StatusText;
    }
}
