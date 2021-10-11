using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    private List<ReplayData> Replay;
    private ReplayData CurrentReplayData;
    private int CurrentReplayDataIterator;
    private int ReplayFrame;
    private bool IsRecording = false;
    private bool IsReplaying = false;
    public PlayerController playerController;
    public ValveController greenValveController, blueValveController;
    public FluidController fluidController;
    public TabletUIController iPadUIController;
    public string ReplayDataPath = "/ReplayData.json";
    // Start is called before the first frame update
    void Start()
    {
        Replay = new List<ReplayData>();
    }

    void FixedUpdate()
    {
        if (IsRecording)
        {
            ReplayEvent positionEvent = playerController.GetPositionReplayEvent();
            ReplayEvent cameraEvent = playerController.GetCameraReplayEvent();
            ReplayEvent greenValveEvent = greenValveController.GetRotationReplayEvent();
            greenValveEvent.Action += " green";
            ReplayEvent blueValveEvent = blueValveController.GetRotationReplayEvent();
            blueValveEvent.Action += " blue";
            List<ReplayEvent> events = new List<ReplayEvent>() { positionEvent, cameraEvent, greenValveEvent, blueValveEvent };
            if (CurrentReplayData == null)
            {
                CurrentReplayData = new ReplayData(events);
            } else if (CurrentReplayData.ReplayEvents != events)
            {
                Replay.Add(CurrentReplayData);
                CurrentReplayData = new ReplayData(events);
            }
            CurrentReplayData.FramesElapsed++;
            Debug.Log($"Added replay data: {positionEvent}, {cameraEvent}, {greenValveEvent}, {blueValveEvent}");
        }
        else if (IsReplaying)
        {
            if (CurrentReplayData == null)
            {
                CurrentReplayData = Replay[0];
                CurrentReplayDataIterator = 0;
                ReplayFrame = 1;
            }
            if (ReplayFrame <= CurrentReplayData.FramesElapsed)
            {
                CallReplay(CurrentReplayData.ReplayEvents);
                ReplayFrame++;
            }
            else if (CurrentReplayDataIterator < Replay.Count)
            {
                CurrentReplayDataIterator++;
                CurrentReplayData = Replay[CurrentReplayDataIterator];
                CallReplay(CurrentReplayData.ReplayEvents);
                ReplayFrame = 2;
            }
            else IsReplaying = false;
        }
    }

    public void StartRecording()
    {
        IsRecording = true;
    }

    public void EndRecording()
    {
        IsRecording = false;
        ReplayDataPackage.SaveReplayData(new ReplayDataPackage(Replay), ReplayDataPath);
        CurrentReplayData = null;
    }

    public void StartReplaying()
    {
        try
        {
            var ReplayPackage = ReplayDataPackage.LoadReplayData(ReplayDataPath);
            Replay = ReplayPackage.ReplayDatas;
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
                    playerController.MoveByRecording(_r);
                    break;
                case "camera":
                    playerController.RotateCameraByRecording(_r);
                    break;
                case "rotation green":
                    greenValveController.RotateByRecording(_r);
                    break;
                case "rotation blue":
                    blueValveController.RotateByRecording(_r);
                    break;
                }
        }
    }
}
