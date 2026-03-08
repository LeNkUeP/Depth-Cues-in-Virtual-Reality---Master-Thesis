using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CueFarTaskDataManager : MonoBehaviour
{
    private string path;
    private int participantID;
    private string beginningTime;
    private string endingTime;

    private Dictionary<string, int> cuePriorities = new Dictionary<string, int>();

    private List<string> taskResults = new List<string>();

    private readonly string[] allDepthCues = new string[]
    {
        "ShadowCast", "ShapeFromShading", "Occlusion", "Disparity", "MotionParallax",
        "AtmosphericPerspective", "RelativeSize", "KnownSize", "HeightInFieldOfView",
        "Accommodation", "Convergence", "ImageBlur", "TextureGradient", "LinearPerspective",
        "Accretion"
    };

    private void Start()
    {
        path = Application.persistentDataPath + "/cue_far_task.csv";
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

        // Depth Cue Header
        foreach (var cue in allDepthCues)
            header += cue + ";";

        // Task Header
        for (int i = 1; i <= allDepthCues.Length; i++)
            header += "Task" + i + ";";

        header += "\n";

        File.WriteAllText(path, header);
    }

    // 1-15
    public void SetCuePriority(string cueName, int priority)
    {
        if (!cuePriorities.ContainsKey(cueName))
            cuePriorities.Add(cueName, priority);
        else
            cuePriorities[cueName] = priority;
    }

    // richtig, falsch, nicht eindeutig
    public void AddTaskResult(string result)
    {
        taskResults.Add(result);
    }

    public void SaveCSV()
    {
        endingTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        string line = participantID + ";" + beginningTime + ";" + endingTime + ";";

        // Cue Prio
        foreach (var cue in allDepthCues)
        {
            if (cuePriorities.ContainsKey(cue))
                line += cuePriorities[cue] + ";";
            else
                line += ";";
        }

        // Task
        for (int i = 0; i < allDepthCues.Length; i++)
        {
            if (i < taskResults.Count)
                line += taskResults[i] + ";";
            else
                line += ";";
        }

        line += "\n";

        File.AppendAllText(path, line);

        Debug.Log("CSV gespeichert: " + path);
    }
}
