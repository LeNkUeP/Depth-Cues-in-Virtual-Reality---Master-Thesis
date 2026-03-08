using System.Collections;
using UnityEngine;

public class DepthCueTogglerRatingPanel : DepthCueTogglerPanel
{
    public GameObject nextButtonUI;

    [Header("Rating Sliders")]
    public GameObject shadowCastRatingSlider;
    public GameObject shapeFromShadingRatingSlider;
    public GameObject occlusionRatingSlider;
    public GameObject disparityRatingSlider;
    public GameObject motionParallaxRatingSlider;
    public GameObject atmosphericPerspectiveRatingSlider;
    public GameObject relativeSizeRatingSlider;
    public GameObject knownSizeRatingSlider;
    public GameObject heightInFieldOfViewRatingSlider;
    public GameObject accommodationRatingSlider;
    public GameObject convergenceRatingSlider;
    public GameObject imageBlurRatingSlider;
    public GameObject textureGradientRatingSlider;
    public GameObject linearPerspectiveRatingSlider;
    public GameObject accretionRatingSlider;
    private GameObject currentRatingSlider;

    private bool shadowCastWasToggled = false;
    private bool shapeFromShadingWasToggled = false;
    private bool occlusionWasToggled = false;
    private bool disparityWasToggled = false;
    private bool motionParallaxWasToggled = false;
    private bool atmosphericPerspectiveWasToggled = false;
    private bool relativeSizeWasToggled = false;
    private bool knownSizeWasToggled = false;
    private bool heightInFieldOfViewWasToggled = false;
    private bool accommodationWasToggled = false;
    private bool convergenceWasToggled = false;
    private bool imageBlurWasToggled = false;
    private bool textureGradientWasToggled = false;
    private bool linearPerspectiveWasToggled = false;
    private bool accretionWasToggled = false;

    public bool CheckIfCompleted()
    {

        if (shadowCastWasToggled && shapeFromShadingWasToggled && occlusionWasToggled && disparityWasToggled &&
            motionParallaxWasToggled && atmosphericPerspectiveWasToggled && relativeSizeWasToggled &&
            knownSizeWasToggled && heightInFieldOfViewWasToggled && accommodationWasToggled && convergenceWasToggled &&
            imageBlurWasToggled && textureGradientWasToggled && linearPerspectiveWasToggled && accretionWasToggled)
        {
            if (!nextButtonUI.activeSelf && transform.localScale == Vector3.one)
            {
                StartCoroutine(ShowUI(nextButtonUI));
            }
            return true;
        }
        return false;
    }

    public void ResetWasToggledState()
    {
        shadowCastWasToggled = false;
        shapeFromShadingWasToggled = false;
        occlusionWasToggled = false;
        disparityWasToggled = false;
        motionParallaxWasToggled = false;
        atmosphericPerspectiveWasToggled = false;
        relativeSizeWasToggled = false;
        knownSizeWasToggled = false;
        heightInFieldOfViewWasToggled = false;
        accommodationWasToggled = false;
        convergenceWasToggled = false;
        imageBlurWasToggled = false;
        textureGradientWasToggled = false;
        linearPerspectiveWasToggled = false;
        accretionWasToggled = false;
    }

    public override void ToggleVisibility()
    {
        base.ToggleVisibility();

        if (openedEyesIconUI.activeSelf)
        {
            if (CheckIfCompleted())
            {
                StartCoroutine(ShowUI(nextButtonUI));
            }
        }
        else
        {
            if (CheckIfCompleted())
            {
                StartCoroutine(HideUI(nextButtonUI));
            }
        }
    }

    public override void ToggleShadowCast()
    {
        base.ToggleShadowCast();
        shadowCastWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(shadowCastRatingSlider);
    }

    public override void ToggleShapeFromShading()
    {
        base.ToggleShapeFromShading();
        shapeFromShadingWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(shapeFromShadingRatingSlider);
    }

    public override void ToggleOcclusion()
    {
        base.ToggleOcclusion();
        occlusionWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(occlusionRatingSlider);
    }

    public override void ToggleDisparity()
    {
        base.ToggleDisparity();
        disparityWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(disparityRatingSlider);
    }

    public override void ToggleMotionParallax()
    {
        base.ToggleMotionParallax();
        motionParallaxWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(motionParallaxRatingSlider);
    }

    public override void ToggleAtmosphericPerspective()
    {
        base.ToggleAtmosphericPerspective();
        atmosphericPerspectiveWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(atmosphericPerspectiveRatingSlider);
    }

    public override void ToggleRelativeSize()
    {
        base.ToggleRelativeSize();
        relativeSizeWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(relativeSizeRatingSlider);
    }

    public override void ToggleKnownSize()
    {
        base.ToggleKnownSize();
        knownSizeWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(knownSizeRatingSlider);
    }

    public override void ToggleHeightInFieldOfView()
    {
        base.ToggleHeightInFieldOfView();
        heightInFieldOfViewWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(heightInFieldOfViewRatingSlider);
    }

    public override void ToggleAccommodation()
    {
        base.ToggleAccommodation();
        accommodationWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(accommodationRatingSlider);
    }

    public override void ToggleConvergence()
    {
        base.ToggleConvergence();
        convergenceWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(convergenceRatingSlider);
    }

    public override void ToggleImageBlur()
    {
        base.ToggleImageBlur();
        imageBlurWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(imageBlurRatingSlider);
    }

    public override void ToggleTextureGradient()
    {
        base.ToggleTextureGradient();
        textureGradientWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(textureGradientRatingSlider);
    }

    public override void ToggleLinearPerspective()
    {
        base.ToggleLinearPerspective();
        linearPerspectiveWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(linearPerspectiveRatingSlider);
    }

    public override void ToggleAccretion()
    {
        base.ToggleAccretion();
        accretionWasToggled = true;
        CheckIfCompleted();
        UpdateRatingSlider(accretionRatingSlider);
    }

    public void UpdateRatingSlider(GameObject slider)
    {
        if (currentRatingSlider != slider)
        {
            // Disable current slider
            if (currentRatingSlider != null)
            {
                currentRatingSlider.SetActive(false);
            }
            // Enable new slider
            currentRatingSlider = slider;
            currentRatingSlider.SetActive(true);
        }
    }

    public void HideCurrentRatingSlider()
    {
        if (currentRatingSlider != null)
        {
            currentRatingSlider.SetActive(false);
        }
    }
}
