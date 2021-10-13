using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    private List<ReplayData> Replay;
    private FinalReplayFrame FinalFrameReplay;
    private ReplayData CurrentReplayData;
    private int CurrentReplayDataIterator;
    private int ReplayFrame;
    private bool IsRecording = false;
    private bool IsReplaying = false;
    public string ReplayDataPath = "/ReplayData.json";

    private PlayerController playerController;
    private ValveController greenValveController, blueValveController;
    private FluidController fluidController;
    private TabletUIController iPadUIController;
    private Scenario scenario;
    // Start is called before the first frame update
    void Start()
    {
        var gm = Camera.main.GetComponent<GameManager>();
        playerController = gm.playerController;
        greenValveController = gm.greenValveController;
        blueValveController = gm.blueValveController;
        fluidController = gm.fluidController;
        iPadUIController = gm.tabletController.tabletUIController;
        scenario = gm.scenario;
        Replay = new List<ReplayData>();
    }

    void FixedUpdate()
    {
        if (IsRecording)
        {
            RecordFrame();
        }
        else if (IsReplaying)
        {
            if (CurrentReplayData == null)
            {
                CurrentReplayData = Replay[0];
                CurrentReplayDataIterator = 1;
                ReplayFrame = 1;
            }
            if (ReplayFrame <= CurrentReplayData.FramesElapsed)
            {
                CallReplay(CurrentReplayData.ReplayEvents);
                ReplayFrame++;
            }
            else if (CurrentReplayDataIterator < Replay.Count)
            {
                CurrentReplayData = Replay[CurrentReplayDataIterator];
                CallReplay(CurrentReplayData.ReplayEvents);
                ReplayFrame = 2;
                CurrentReplayDataIterator++;
            }
            else
            {
                IsReplaying = false;
                SetLastFrame(FinalFrameReplay);
            }
        }
    }

    public void StartRecording()
    {
        IsRecording = true;
    }

    public void EndRecording()
    {
        if (IsRecording)
        {
            IsRecording = false;
            RecordFrame();
            ReplayDataPackage.SaveReplayData(new ReplayDataPackage(Replay, RecordLastFrameData()), ReplayDataPath);
            CurrentReplayData = null;
        }
    }

    public void StartReplaying()
    {
        CurrentReplayData = null;
        try
        {
            var ReplayPackage = ReplayDataPackage.LoadReplayData(ReplayDataPath);
            Replay = ReplayPackage.ReplayDatas;
            FinalFrameReplay = ReplayPackage.FinalFrame;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting file: {e}");
            return;
        }
        if (Replay == null)
        {
            Debug.LogError("Replay file not found!");
            return;
        }
        if (Replay.Count <= 0)
        {
            Debug.LogError("Replay file is empty!");
            return;
        }
        IsReplaying = true;
        fluidController.ResetFluid();
        iPadUIController.ChangeState(TabletUIController.UIStates.InProgress);
        return;
    }

    public void CallReplay(List<ReplayEvent> events)
    {
        foreach (ReplayEvent _r in events) {
            switch (_r.Action) {
                case "movement":
                    playerController.MoveByRecording(_r.Data);
                    break;
                case "camera":
                    playerController.RotateCameraByRecording(_r.Data);
                    break;
                case "rotation green":
                    greenValveController.RotateByRecording(_r.Data.x);
                    break;
                case "rotation blue":
                    blueValveController.RotateByRecording(_r.Data.x);
                    break;
                }
        }
    }

    private void RecordFrame()
    {
        ReplayEvent positionEvent = new ReplayEvent("movement", playerController.GetPositionReplayEvent());
        ReplayEvent cameraEvent = new ReplayEvent("camera", Camera.main.transform.rotation);
        ReplayEvent greenValveEvent = greenValveController.GetRotationReplayEvent();
        greenValveEvent.Action += " green";
        ReplayEvent blueValveEvent = blueValveController.GetRotationReplayEvent();
        blueValveEvent.Action += " blue";
        List<ReplayEvent> events = new List<ReplayEvent>() { positionEvent, cameraEvent, greenValveEvent, blueValveEvent };
        if (CurrentReplayData == null)
        {
            CurrentReplayData = new ReplayData(events);
        }
        else
        if (!System.Linq.Enumerable.SequenceEqual(CurrentReplayData.ReplayEvents, events))
        {
            Replay.Add(CurrentReplayData);
            CurrentReplayData = new ReplayData(events);
        }
        CurrentReplayData.FramesElapsed++;
        //Debug.Log($"Added replay data: position  {positionEvent.Data}, camera {cameraEvent.Data}, green {greenValveEvent.Data}, blue {blueValveEvent.Data}");
    }

    private FinalReplayFrame RecordLastFrameData()
    {
        FinalReplayFrame replayFrame = new FinalReplayFrame();
        replayFrame.Position = playerController.GetPositionReplayEvent();
        replayFrame.Camera = Camera.main.transform.rotation.eulerAngles;
        replayFrame.OverallFluid = fluidController.GetFluidVolume();
        replayFrame.FluidRatio = fluidController.GetGreenToBlueFluidRatio();
        replayFrame.ActionsTaken = scenario.GetValveActions();
        replayFrame.Time = scenario.GetElapsedTime();
        return replayFrame;
    }

    private void SetLastFrame(FinalReplayFrame _f)
    {
        playerController.MoveByRecording(_f.Position);
        playerController.RotateCameraByRecording(_f.Camera);
        fluidController.ChangeColour(_f.FluidRatio);
        fluidController.Fill(_f.OverallFluid);
        greenValveController.RotateByRecording(0);
        blueValveController.RotateByRecording(0);
        scenario.SetElapsedTime(_f.Time);
        greenValveController.SetValveActions(_f.ActionsTaken);
        Camera.main.GetComponent<Scenario>().EndScenario();
    }
}
