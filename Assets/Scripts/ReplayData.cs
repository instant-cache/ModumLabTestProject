using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReplayDataPackage
{
    public List<ReplayData> ReplayDatas;

    public static bool SaveReplayData(ReplayDataPackage _data, string _path)
    {
        string json = JsonUtility.ToJson(_data);
        System.IO.File.WriteAllText(Application.persistentDataPath + _path, json);
        Debug.Log("Recorded replay to " + Application.persistentDataPath + _path);
        return true;
    }

    public static ReplayDataPackage LoadReplayData(string _path)
    {
        string json = System.IO.File.ReadAllText(Application.persistentDataPath + _path);
        ReplayDataPackage _data = JsonUtility.FromJson<ReplayDataPackage>(json);
        return _data;
    }

    public ReplayDataPackage(List<ReplayData> replayDatas)
    {
        ReplayDatas = replayDatas;
    }
}

[Serializable]
public class ReplayData
{
    public int FramesElapsed;
    public List<ReplayEvent> ReplayEvents;

    public ReplayData(List<ReplayEvent> replayEvents)
    {
        ReplayEvents = replayEvents;
    }
}

[Serializable]
public class ReplayEvent
{
    public string Action;
    public Vector3 Data;

    public ReplayEvent(string action, Vector3 vectorData){
        Action = action;
        Data = vectorData;
    }

    public ReplayEvent(string action, Quaternion rotationData)
    {
        Action = action;
        Data = rotationData.eulerAngles;
    }

    public ReplayEvent(string action, float angleData)
    {
        Action = action;
        Data = new Vector3(angleData, 0, 0);
    }
}

public enum ReplayActionTypes
{
    CameraMovement,
    PlayerMovement,
    ValveRotation
}