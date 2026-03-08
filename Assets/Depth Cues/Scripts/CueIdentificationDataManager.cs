using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CueIdentificationDataManager : MonoBehaviour
{
    private string path;
    private int participantID;
    private string beginningTime;
    private string endingTime;
    private string currentDisabledCue;
    private Dictionary<string, string> results = new Dictionary<string, string>();
    private readonly string[] allDepthCues = new string[]
    {
        "ShadowCast", "ShapeFromShading", "Occlusion", "Disparity", "MotionParallax",
        "AtmosphericPerspective", "RelativeSize", "KnownSize", "HeightInFieldOfView",
        "Accommodation", "Convergence", "ImageBlur", "TextureGradient", "LinearPerspective",
        "Accretion"
    };

    private void Start()
    {
        path = Application.persistentDataPath + "/cue_identification.csv";
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

        header += "\n";

        File.WriteAllText(path, header);
    }

    public void SetDisabledCue(string disabledCue)
    {
        currentDisabledCue = disabledCue;
    }

    public void SetChoice(string selectedCue)
    {
        if (string.IsNullOrEmpty(currentDisabledCue))
        {
            Debug.LogError("Kein deaktivierter Cue vorhanden!");
            return;
        }

        results[currentDisabledCue] = selectedCue;
        currentDisabledCue = null;
    }

    public void SaveCSV()
    {
        endingTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        string line = participantID + ";" + beginningTime + ";" + endingTime + ";";

        foreach (var cue in allDepthCues)
        {
            if (results.ContainsKey(cue))
                line += results[cue] + ";";
            else
                line += ";";
        }

        line += "\n";

        File.AppendAllText(path, line);

        Debug.Log("CSV gespeichert: " + path);
    }
}