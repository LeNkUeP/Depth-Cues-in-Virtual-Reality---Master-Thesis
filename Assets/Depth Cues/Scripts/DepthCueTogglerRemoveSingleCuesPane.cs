using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DepthCueTogglerRemoveSingleCuesPane : DepthCueTogglerPanel
{
    public GameObject nextButtonUI;
    public CueFarTaskDataManager cueFarTaskDataManager;
    public CueNearTaskDataManager cueNearTaskDataManager;
    public bool farTask = true;

    public void DisableActiveToggle()
    {
        foreach (Toggle t in allToggles)
        {
            if (t != null && t.isOn)
            { 
                if (farTask)
                {
                    cueFarTaskDataManager.SetCuePriority(t.name, CountActiveToggles());
                }
                else
                {
                    cueNearTaskDataManager.SetCuePriority(t.name, CountActiveToggles());
                }
                t.gameObject.SetActive(false);
                t.SetIsOnWithoutNotify(false);
                break;                        
            }
        }
    }

    public int CountActiveToggles()
    {
        int count = 0;

        foreach (Toggle t in allToggles)
        {
            if (t != null && t.gameObject.activeSelf)
            {
                count++;
            }
        }

        return count;
    }

    public void ResetTogglesAndCues()
    {
        foreach (Toggle t in allToggles)
        {
            if (t != null && !t.isOn)
            {
                t.gameObject.SetActive(true);
                t.isOn = true;
                break;
            }
        }
    }

    public bool AllTogglesDeactivated()
    {
        foreach (Toggle t in allToggles)
        {
            if (t != null && t.gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    public void CheckNextButton()
    {
        if(IsButtonSelected())
        {
            nextButtonUI.SetActive(true);
        }
        else
        {
            nextButtonUI.SetActive(false);
        }
    }

    public override void ToggleShadowCast()
    {
        base.ToggleShadowCast();
        CheckNextButton();
    }

    public override void ToggleShapeFromShading()
    {
        base.ToggleShapeFromShading();
        CheckNextButton();
    }

    public override void ToggleOcclusion()
    {
        base.ToggleOcclusion();
        CheckNextButton();
    }

    public override void ToggleDisparity()
    {
        base.ToggleDisparity();
        CheckNextButton();
    }

    public override void ToggleMotionParallax()
    {
        base.ToggleMotionParallax();
        CheckNextButton();
    }

    public override void ToggleAtmosphericPerspective()
    {
        base.ToggleAtmosphericPerspective();
        CheckNextButton();
    }

    public override void ToggleRelativeSize()
    {
        base.ToggleRelativeSize();
        CheckNextButton();
    }

    public override void ToggleKnownSize()
    {
        base.ToggleKnownSize();
        CheckNextButton();
    }

    public override void ToggleHeightInFieldOfView()
    {
        base.ToggleHeightInFieldOfView();
        CheckNextButton();
    }

    public override void ToggleAccommodation()
    {
        base.ToggleAccommodation();
        CheckNextButton();
    }

    public override void ToggleConvergence()
    {
        base.ToggleConvergence();
        CheckNextButton();
    }

    public override void ToggleImageBlur()
    {
        base.ToggleImageBlur();
        CheckNextButton();
    }

    public override void ToggleTextureGradient()
    {
        base.ToggleTextureGradient();
        CheckNextButton();
    }

    public override void ToggleLinearPerspective()
    {
        base.ToggleLinearPerspective();
        CheckNextButton();
    }

    public override void ToggleAccretion()
    {
        base.ToggleAccretion();
        CheckNextButton();
    }
}
