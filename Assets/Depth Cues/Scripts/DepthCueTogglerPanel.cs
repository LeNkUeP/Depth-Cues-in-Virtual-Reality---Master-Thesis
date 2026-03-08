using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DepthCueTogglerPanel : DepthCueToggler
{
    [Header("UI")]
    public GameObject openedEyesIconUI;
    public GameObject closedEyesIconUI;
    
    public Toggle shadowCastToggle;
    public Toggle shapeFromShadingToggle;
    public Toggle occlusionToggle;
    public Toggle disparityToggle;
    public Toggle motionParallaxToggle;
    public Toggle atmosphericPerspectiveToggle;
    public Toggle relativeSizeToggle;
    public Toggle knownSizeToggle;
    public Toggle heightInFieldOfViewToggle;
    public Toggle accommodationToggle;
    public Toggle convergenceToggle;
    public Toggle imageBlurToggle;
    public Toggle textureGradientToggle;
    public Toggle linearPerspectiveToggle;
    public Toggle accretionToggle;
    public Toggle[] allToggles;

    private void Awake()
    {
        allToggles = new Toggle[]
        {
            shadowCastToggle,
            shapeFromShadingToggle,
            occlusionToggle,
            disparityToggle,
            motionParallaxToggle,
            atmosphericPerspectiveToggle,
            relativeSizeToggle,
            knownSizeToggle,
            heightInFieldOfViewToggle,
            accommodationToggle,
            convergenceToggle,
            imageBlurToggle,
            textureGradientToggle,
            linearPerspectiveToggle,
            accretionToggle
        };
    }

    public void DeactivateAllUIToggles()
    {
        foreach (Toggle t in allToggles)
        {
            if (t != null)
                t.gameObject.SetActive(false);
        }
    }

    public void ActivateAllUIToggles()
    {
        foreach (Toggle t in allToggles)
        {
            if (t != null)
                t.gameObject.SetActive(true);
        }
    }

    public bool IsButtonSelected()
    {
        foreach (Toggle t in allToggles)
        {
            if (t != null && t.isOn)
                return true;
        }
        return false;
    }

    public override void ToggleShadowCast()
    {
        base.ToggleShadowCast();
    }

    public override void ToggleShapeFromShading()
    {
        base.ToggleShapeFromShading();
    }

    public override void ToggleOcclusion()
    {
        base.ToggleOcclusion();
    }

    public override void ToggleDisparity()
    {
        base.ToggleDisparity();
    }

    public override void ToggleMotionParallax()
    {
        base.ToggleMotionParallax();
    }

    public override void ToggleAtmosphericPerspective()
    {
        base.ToggleAtmosphericPerspective();
    }

    public override void ToggleRelativeSize()
    {
        base.ToggleRelativeSize();
    }

    public override void ToggleKnownSize()
    {
        base.ToggleKnownSize();
    }

    public override void ToggleHeightInFieldOfView()
    {
        base.ToggleHeightInFieldOfView();
    }

    public override void ToggleAccommodation()
    {
        base.ToggleAccommodation();
    }

    public override void ToggleConvergence()
    {
        base.ToggleConvergence();
    }

    public override void ToggleImageBlur()
    {
        base.ToggleImageBlur();
    }

    public override void ToggleTextureGradient()
    {
        base.ToggleTextureGradient();
    }

    public override void ToggleLinearPerspective()
    {
        base.ToggleLinearPerspective();
    }

    public override void ToggleAccretion()
    {
        base.ToggleAccretion();
    }

    public virtual void ToggleVisibility()
    {
        // normal eye = visible, closed/crossed eye = invisible
        SwitchHideUIEyeIcons();

        // only hide do not dedactivate this gameobject, update needs to run
        if (openedEyesIconUI.activeSelf)
        {
            StartCoroutine(ShowUI(gameObject));
        }
        else
        {
            StartCoroutine(HideUIButKeepActive(gameObject));
        }
    }

    public void SwitchHideUIEyeIcons()
    {
        openedEyesIconUI.SetActive(!openedEyesIconUI.activeSelf);
        closedEyesIconUI.SetActive(!closedEyesIconUI.activeSelf);
    }

    public IEnumerator ShowUI(GameObject objectToShow)
    {
        objectToShow.SetActive(true);
        objectToShow.GetComponent<Animator>().SetTrigger("show");
        yield return null;
    }

    public IEnumerator HideUIButKeepActive(GameObject objectToHide)
    {
        objectToHide.GetComponent<Animator>().SetTrigger("hide");
        yield return null;
    }

    public IEnumerator HideUI(GameObject objectToHide)
    {
        objectToHide.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(.5f);
        objectToHide.SetActive(false);
    }
}
