using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CueNearTaskDataManager : MonoBehaviour
{
    private string path;
    private int participantID;
    private string beginningTime;
    private string endingTime;

    private Dictionary<string, int> cuePriorities = new Dictionary<string, int>();

    private List<float> taskDurations = new List<float>();
    private List<float> taskScores = new List<float>();

    private readonly string[] allDepthCues = new string[]
    {
        "ShadowCast","ShapeFromShading","Occlusion","Disparity","MotionParallax",
        "AtmosphericPerspective","RelativeSize","KnownSize","HeightInFieldOfView",
        "Accommodation","Convergence","ImageBlur","TextureGradient","LinearPerspective",
        "Accretion"
    };

    private void Start()
    {
        path = Application.persistentDataPath + "/cue_near_task.csv";
        participantID = GetNextID(path);
        beginningTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        if (!File.Exists(path))
            WriteHeader();
    }

    private int GetNextID(string path)
    {
        if (!File.Exists(path))
            return 1;

        return File.ReadAllLines(path).Length;
    }

    private void WriteHeader()
    {
        string header = "ParticipantID;BeginningTime;EndingTime;";

        foreach (var cue in allDepthCues)
            header += cue + ";";

        for (int i = 1; i <= allDepthCues.Length; i++)
        {
            header += "Task" + i + "Duration;";
            header += "Task" + i + "Success;";
        }

        header += "\n";

        File.WriteAllText(path, header);
    }

    public void SetCuePriority(string cueName, int priority)
    {
        cuePriorities[cueName] = priority;
    }

    public void AddTaskResult(float durationSeconds, float successScore)
    {
        taskDurations.Add(durationSeconds);
        taskScores.Add(successScore);
    }

    public void SaveCSV()
    {
        endingTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        string line = participantID + ";" + beginningTime + ";" + endingTime + ";";

        foreach (var cue in allDepthCues)
        {
            if (cuePriorities.ContainsKey(cue))
                line += cuePriorities[cue] + ";";
            else
                line += ";";
        }

        for (int i = 0; i < allDepthCues.Length; i++)
        {
            if (i < taskDurations.Count)
                line += taskDurations[i] + ";";
            else
                line += ";";

            if (i < taskScores.Count)
                line += taskScores[i] + ";";
            else
                line += ";";
        }

        line += "\n";

        File.AppendAllText(path, line);

        Debug.Log("Near CSV gespeichert: " + path);
    }
}