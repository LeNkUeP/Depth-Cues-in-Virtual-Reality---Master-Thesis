using System.Collections.Generic;
using UnityEngine;

public class RandomDepthCueDisabler : MonoBehaviour
{
    public DepthCueToggler depthCueToggler;
    public CueIdentificationDataManager identificationDataManager;

    private List<string> remainingCues = new List<string>();

    public void InitCues()
    {
        remainingCues.AddRange(new string[]
        {
            "ShadowCast", "ShapeFromShading", "Occlusion", "Disparity", "MotionParallax",
            "AtmosphericPerspective", "RelativeSize", "KnownSize", "HeightInFieldOfView",
            "Accommodation", "Convergence", "ImageBlur", "TextureGradient", "LinearPerspective",
            "Accretion"
        });
    }

    public string DisableRandomDepthCue()
    {
        if (remainingCues.Count == 0) return null;

        int index = Random.Range(0, remainingCues.Count);
        string cue = remainingCues[index];
        remainingCues.RemoveAt(index);

        switch (cue)
        {
            case "ShadowCast": depthCueToggler.ToggleShadowCast(); break;
            case "ShapeFromShading": depthCueToggler.ToggleShapeFromShading(); break;
            case "Occlusion": depthCueToggler.ToggleOcclusion(); break;
            case "Disparity": depthCueToggler.ToggleDisparity(); break;
            case "MotionParallax": depthCueToggler.ToggleMotionParallax(); break;
            case "AtmosphericPerspective": depthCueToggler.ToggleAtmosphericPerspective(); break;
            case "RelativeSize": depthCueToggler.ToggleRelativeSize(); break;
            case "KnownSize": depthCueToggler.ToggleKnownSize(); break;
            case "HeightInFieldOfView": depthCueToggler.ToggleHeightInFieldOfView(); break;
            case "Accommodation": depthCueToggler.ToggleAccommodation(); break;
            case "Convergence": depthCueToggler.ToggleConvergence(); break;
            case "ImageBlur": depthCueToggler.ToggleImageBlur(); break;
            case "TextureGradient": depthCueToggler.ToggleTextureGradient(); break;
            case "LinearPerspective": depthCueToggler.ToggleLinearPerspective(); break;
            case "Accretion": depthCueToggler.ToggleAccretion(); break;
        }

        identificationDataManager.SetDisabledCue(cue);

        return cue;
    }

    public bool HasRemainingCues()
    {
        return remainingCues.Count > 0;
    }
}