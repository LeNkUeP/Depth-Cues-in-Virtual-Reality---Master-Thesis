using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StudyDataManager : MonoBehaviour
{
    [Header("General")]

    [Header("Phase 2 Sliders")]
    public Slider shadowCastRatingSlider;
    public Slider shapeFromShadingRatingSlider;
    public Slider occlusionRatingSlider;
    public Slider disparityRatingSlider;
    public Slider motionParallaxRatingSlider;
    public Slider atmosphericPerspectiveRatingSlider;
    public Slider relativeSizeRatingSlider;
    public Slider knownSizeRatingSlider;
    public Slider heightInFieldOfViewRatingSlider;
    public Slider accommodationRatingSlider;
    public Slider convergenceRatingSlider;
    public Slider imageBlurRatingSlider;
    public Slider textureGradientRatingSlider;
    public Slider linearPerspectiveRatingSlider;
    public Slider accretionRatingSlider;

    private string beginningTime;
    private string endingTime;
    private int participantID;
    private string path;

    private void Start()
    {
        path = Application.persistentDataPath + "/study_results.csv";
        bool fileExists = File.Exists(path);

        participantID = GetNextID(path);
        beginningTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        if (!fileExists)
        {
            WriteHeader();
        }
    }

    public static int GetNextID(string path)
    {
        if (!File.Exists(path))
        {
            return 1;
        }

        int lineCount = File.ReadAllLines(path).Length;
        // header included
        return lineCount;
    }

    public string GetDepthCueValues()
    {
        return shadowCastRatingSlider.value + ";" +
               shapeFromShadingRatingSlider.value + ";" +
               occlusionRatingSlider.value + ";" +
               disparityRatingSlider.value + ";" +
               motionParallaxRatingSlider.value + ";" +
               atmosphericPerspectiveRatingSlider.value + ";" +
               relativeSizeRatingSlider.value + ";" +
               knownSizeRatingSlider.value + ";" +
               heightInFieldOfViewRatingSlider.value + ";" +
               accommodationRatingSlider.value + ";" +
               convergenceRatingSlider.value + ";" +
               imageBlurRatingSlider.value + ";" +
               textureGradientRatingSlider.value + ";" +
               linearPerspectiveRatingSlider.value + ";" +
               accretionRatingSlider.value;
    }

    private void WriteHeader()
    {
        string header = "Participant ID;Beginning time;Ending time;" +
                        "Shadow cast rating;Shape from shading rating;Occlusion rating;Disparity rating;" +
                        "Motion parallax rating;Atmospheric perspective rating;Relative size rating;" +
                        "Known size rating;Height in field of view rating;Accommodation rating;Convergence rating;" +
                        "Image blur rating;Texture gradient rating;Linear perspective rating;Accretion rating\n";

        File.WriteAllText(path, header);
    }

    public void SaveCSV()
    {
        endingTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        string line = participantID + ";" +
                      beginningTime + ";" +
                      endingTime + ";" +
                      GetDepthCueValues() + "\n";

        File.AppendAllText(path, line);
    }
}
