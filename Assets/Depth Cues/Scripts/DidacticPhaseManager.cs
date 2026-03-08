using Oculus.Interaction;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class DidacticPhaseManager: MonoBehaviour
{
    [Header("Managers & Controllers")]
    public EnvironmentAnimationController environmentAnimationController;
    public CueRatingDataManager cueRatingDataManager;
    public CueIdentificationDataManager identificationDataManager;
    public CueFarTaskDataManager farTaskDataManager;
    public CueNearTaskDataManager nearTaskDataManager;

    [Header("Scene Environments")]
    public GameObject phase2Environment;
    public GameObject phase3FarEnvironment;
    public GameObject phase3NearEnvironment;

    [Header("General UI")]
    public GameObject startButtonUI;
    public GameObject nextButtonUI;

    [Header("Intro UI")]
    public GameObject welcomeUI;
    public GameObject explanationDepthCuesUI;
    public GameObject explanationBinoAndMonoUI;
    public GameObject explanationDidacticPartUI;
    public GameObject explanationTasksUI;

    [Header("Phase 1 UI")]
    public GameObject phase1IntroUI;
    public GameObject phase1DidacticDepthCuePanelUI;

    [Header("Phase 2 UI")]
    public GameObject phase2IntroUI;
    public GameObject phase2AdditionalRatingTaskUI;
    public GameObject phase2DepthCueTogglerPanelUI;
    public GameObject phase2hidePanelButtonUI;
    public GameObject phase2IdentifyTaskExplanationUI;
    public GameObject phase2DepthCueChoicePanelUI;

    [Header("Phase3 UI")]
    public GameObject phase3IntroUI;
    public GameObject phase3TaskExplanationUI;
    public GameObject phase3LinearPerspectiveExplanationUI;
    public GameObject phase3FarDepthCueTogglerPanelUI;
    public GameObject phase3FarHidePanelButtonUI;
    public GameObject phase3FarTaskUI;
    public GameObject phase3FarEnterButtonUI;
    public GameObject phase3NearTaskExplanationUI;
    public GameObject phase3NearTaskAccretionExplanationUI;
    public GameObject phase3NearDepthCueTogglerPanelUI;
    public GameObject phase3NearHidePanelButtonUI;
    public GameObject phase3NearTaskUI;
    public GameObject finalUI;

    private DidacticDepthCuePanel phase1DidacticDepthCuePanel;
    private DepthCueTogglerRatingPanel phase2DepthCueTogglerPanel;
    private DepthCueChoicePanel phase2DepthCueChoicePanel;
    private DepthCueTogglerRemoveSingleCuesPane phase3FarDepthCueTogglerPanel;
    private DepthCueTogglerRemoveSingleCuesPane phase3NearDepthCueTogglerPanel;

    private void Start()
    {
        EnableAllEnvironments();
        PreGameInitializations();
    }

    public void EnableAllEnvironments()
    {
        phase2Environment.SetActive(true);
        phase3FarEnvironment.SetActive(true);
        phase3NearEnvironment.SetActive(true);
    }

    private void DisableAllUI()
    {
        startButtonUI.SetActive(false);
        welcomeUI.SetActive(false);
        nextButtonUI.SetActive(false);
        explanationDepthCuesUI.SetActive(false);
        explanationBinoAndMonoUI.SetActive(false);
        explanationDidacticPartUI.SetActive(false);
        explanationTasksUI.SetActive(false);
        phase1IntroUI.SetActive(false);
        phase1DidacticDepthCuePanelUI.SetActive(false);
        phase2IntroUI.SetActive(false);
        phase2AdditionalRatingTaskUI.SetActive(false);
        phase2DepthCueTogglerPanelUI.SetActive(false);
        phase2hidePanelButtonUI.SetActive(false);
        phase2IdentifyTaskExplanationUI.SetActive(false);
        phase3IntroUI.SetActive(false);
        phase3TaskExplanationUI.SetActive(false);
        phase3LinearPerspectiveExplanationUI.SetActive(false);
        phase3FarDepthCueTogglerPanelUI.SetActive(false);
        phase3FarTaskUI.SetActive(false);
        phase3FarEnterButtonUI.SetActive(false);
        phase3NearTaskExplanationUI.SetActive(false);
        phase3NearTaskAccretionExplanationUI.SetActive(false);
        phase3NearDepthCueTogglerPanelUI.SetActive(false);
        phase3NearTaskUI.SetActive(false);
        finalUI.SetActive(false);
    }

    private void PreGameInitializations()
    {
        DisableAllUI();
        environmentAnimationController.DisableAllObjects(phase2Environment);
        environmentAnimationController.DisableAllObjects(phase3FarEnvironment);
        environmentAnimationController.DisableAllObjects(phase3NearEnvironment);

        // Must be active to complete initialization process
        phase1DidacticDepthCuePanelUI.SetActive(true);
        phase2DepthCueTogglerPanelUI.SetActive(true);
        phase3FarDepthCueTogglerPanelUI.SetActive(true);
        phase3NearDepthCueTogglerPanelUI.SetActive(true);

        phase1DidacticDepthCuePanel = phase1DidacticDepthCuePanelUI.GetComponent<DidacticDepthCuePanel>();
        phase2DepthCueTogglerPanel = phase2DepthCueTogglerPanelUI.GetComponent<DepthCueTogglerRatingPanel>();
        phase2DepthCueChoicePanel = phase2DepthCueChoicePanelUI.GetComponent<DepthCueChoicePanel>();
        phase3FarDepthCueTogglerPanel = phase3FarDepthCueTogglerPanelUI.GetComponent<DepthCueTogglerRemoveSingleCuesPane>();
        phase3NearDepthCueTogglerPanel = phase3NearDepthCueTogglerPanelUI.GetComponent<DepthCueTogglerRemoveSingleCuesPane>();

        // only hide visually
        phase2DepthCueTogglerPanel.ToggleVisibility();
        phase2DepthCueTogglerPanel.SwitchHideUIEyeIcons();
        phase3FarDepthCueTogglerPanel.ToggleVisibility();
        phase3FarDepthCueTogglerPanel.SwitchHideUIEyeIcons();
        phase3NearDepthCueTogglerPanel.ToggleVisibility();
        phase3NearDepthCueTogglerPanel.SwitchHideUIEyeIcons();

        // Prevents lag/initialization in moment of usage
        phase2DepthCueTogglerPanel.ToggleAtmosphericPerspective();
        phase2DepthCueTogglerPanel.ToggleAtmosphericPerspective();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleLinearPerspective();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleLinearPerspective();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleShapeFromShading();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleShapeFromShading();

        // keep active to preload videos and prevent lag
        phase1DidacticDepthCuePanel.ToggleVisibility();

        // Toggle depth cue panel slider should be invisible when no cue was selected
        phase2DepthCueTogglerPanelUI.GetComponent<DepthCueTogglerRatingPanel>().HideCurrentRatingSlider();
        phase2DepthCueTogglerPanelUI.GetComponent<DepthCueTogglerRatingPanel>().ResetWasToggledState();

        // first only start button should be visible to start prototype
        startButtonUI.SetActive(true);
        startButtonUI.GetComponent<Animator>().SetTrigger("pulse");
    }

    private void ShowKeyboard()
    {
        TouchScreenKeyboard overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    private IEnumerator SwitchUI(GameObject old, GameObject next)
    {
        old.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(.5f);
        old.SetActive(false);
        next.SetActive(true);
        next.GetComponent<Animator>().SetTrigger("show");
    }

    private IEnumerator SwitchUIButKeepActive(GameObject old, GameObject next)
    {
        old.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(.5f);
        next.SetActive(true);
        next.GetComponent<Animator>().SetTrigger("show");
    }

    private IEnumerator DelayedShowUI(GameObject objectToShow)
    {
        yield return new WaitForSeconds(.5f);
        objectToShow.SetActive(true);
        objectToShow.GetComponent<Animator>().SetTrigger("show");
    }

    private IEnumerator HideUI(GameObject objectToHide)
    {
        objectToHide.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(.5f);
        objectToHide.SetActive(false);
    }

    private IEnumerator HideUIButKeepActive(GameObject objectToHide)
    {
        objectToHide.GetComponent<Animator>().SetTrigger("hide");
        yield return null;
    }

    private IEnumerator ShowUI(GameObject objectToShow)
    {
        objectToShow.SetActive(true);
        objectToShow.GetComponent<Animator>().SetTrigger("show");
        yield return null;
    }

    public void StartInitialPhase()
    {
        StartCoroutine(SwitchUI(startButtonUI, welcomeUI));
        StartCoroutine(DelayedShowUI(nextButtonUI));
    }

    public void Next()
    {
        // PHASE 1: DIDACTIC
        if (welcomeUI.activeSelf)
        {
            StartCoroutine(SwitchUI(welcomeUI, explanationDepthCuesUI));
        }
        else if (explanationDepthCuesUI.activeSelf)
        {
            StartCoroutine(SwitchUI(explanationDepthCuesUI, explanationBinoAndMonoUI));
        }
        else if (explanationBinoAndMonoUI.activeSelf)
        {
            StartCoroutine(SwitchUI(explanationBinoAndMonoUI, explanationDidacticPartUI));
        }
        else if (explanationDidacticPartUI.activeSelf)
        {
            StartCoroutine(SwitchUI(explanationDidacticPartUI, explanationTasksUI)); 
        }
        else if (explanationTasksUI.activeSelf)
        {
            StartCoroutine(SwitchUI(explanationTasksUI, phase1IntroUI));
        }
        else if (phase1IntroUI.activeSelf)
        {
            phase1DidacticDepthCuePanelUI.GetComponentInChildren<Canvas>().enabled = true;
            phase1DidacticDepthCuePanelUI.GetComponent<PokeInteractable>().enabled = true;
            phase1DidacticDepthCuePanelUI.SetActive(false);

            StartCoroutine(HideUI(nextButtonUI));
            StartCoroutine(SwitchUI(phase1IntroUI, phase1DidacticDepthCuePanelUI));
        }
        //PHASE 2: DEPTH CUE EFFECTS
        else if (phase1DidacticDepthCuePanelUI.activeSelf)
        {
            phase1DidacticDepthCuePanel.HideTextAndVideoUI();

            StartCoroutine(SwitchUI(phase1DidacticDepthCuePanelUI, phase2IntroUI));
        }
        else if (phase2IntroUI.activeSelf)
        {
            StartCoroutine(SwitchUI(phase2IntroUI, phase2AdditionalRatingTaskUI));
        }
        else if (phase2AdditionalRatingTaskUI.activeSelf)
        {
            StartCoroutine(HideUI(nextButtonUI));
            StartCoroutine(SwitchUI(phase2AdditionalRatingTaskUI, phase2DepthCueTogglerPanelUI));
            phase2DepthCueTogglerPanel.SetStartingCueState();
            StartCoroutine(DelayedShowUI(phase2hidePanelButtonUI));

            // show scene objects
            environmentAnimationController.AnimateShowObjects(phase2Environment);
        }
        else if (phase2DepthCueTogglerPanelUI.activeSelf && phase2DepthCueTogglerPanelUI.transform.localScale == Vector3.one)
        {
            phase2DepthCueTogglerPanel.EnableAllCues();

            StartCoroutine(HideUI(phase2hidePanelButtonUI));
            StartCoroutine(SwitchUIButKeepActive(phase2DepthCueTogglerPanelUI, phase2IdentifyTaskExplanationUI));

            // save ratings
            cueRatingDataManager.SaveCSV();
        }
        else if (phase2IdentifyTaskExplanationUI.activeSelf)
        {
            StartCoroutine(HideUI(nextButtonUI));
            StartCoroutine(SwitchUI(phase2IdentifyTaskExplanationUI, phase2DepthCueChoicePanelUI));
        }
        else if (phase2DepthCueChoicePanelUI.activeSelf)
        {
            // Unnatural depth cues need to be resetted/deactivated
            phase2DepthCueTogglerPanel.RestoreNormalView();
            environmentAnimationController.AnimateHideObjects(phase2Environment);
            StartCoroutine(SwitchUI(phase2DepthCueChoicePanelUI, phase3IntroUI));
            StartCoroutine(ShowUI(nextButtonUI));
            phase2DepthCueTogglerPanelUI.SetActive(false);

            // save identification
            identificationDataManager.SaveCSV();
        }
        else if (phase3IntroUI.activeSelf)
        {
            StartCoroutine(SwitchUI(phase3IntroUI, phase3TaskExplanationUI));
        }
        else if (phase3TaskExplanationUI.activeSelf)
        {
            StartCoroutine(SwitchUI(phase3TaskExplanationUI, phase3LinearPerspectiveExplanationUI));
        }
        else if (phase3LinearPerspectiveExplanationUI.activeSelf)
        {
            StartCoroutine(SwitchUI(phase3LinearPerspectiveExplanationUI, phase3FarTaskUI));
            StartCoroutine(HideUI(nextButtonUI));
            environmentAnimationController.AnimateShowObjects(phase3FarEnvironment);
            phase3FarDepthCueTogglerPanel.SetStartingCueState();
            phase3FarDepthCueTogglerPanel.EnableAllCues();
        }
        else if (phase3FarDepthCueTogglerPanelUI.activeSelf && phase3FarDepthCueTogglerPanelUI.transform.localScale == Vector3.one)
        {
            phase3FarDepthCueTogglerPanel.DisableActiveToggle();
            StartCoroutine(SwitchUIButKeepActive(phase3FarDepthCueTogglerPanelUI, phase3FarTaskUI));
            StartCoroutine(HideUI(nextButtonUI));
            StartCoroutine(HideUI(phase3FarHidePanelButtonUI));
        }
        else if (phase3FarTaskUI.activeSelf)
        {
            // All toggles gone -> Start next task and reset panel
            if (phase3FarDepthCueTogglerPanel.AllTogglesDeactivated())
            {
                environmentAnimationController.AnimateHideObjects(phase3FarEnvironment);
                phase3FarDepthCueTogglerPanel.ResetTogglesAndCues();
                phase3FarDepthCueTogglerPanel.RestoreNormalView();
                StartCoroutine(SwitchUI(phase3FarTaskUI, phase3NearTaskExplanationUI));
                StartCoroutine(ShowUI(nextButtonUI));
                phase3FarDepthCueTogglerPanelUI.SetActive(false);

                // save results
                farTaskDataManager.SaveCSV();
            }
            else
            {
                StartCoroutine(SwitchUI(phase3FarTaskUI, phase3FarDepthCueTogglerPanelUI));
                StartCoroutine(HideUI(nextButtonUI));
            }
        }
        else if (phase3NearTaskExplanationUI.activeSelf)
        {
            StartCoroutine(SwitchUI(phase3NearTaskExplanationUI, phase3NearTaskAccretionExplanationUI));
        }
        else if (phase3NearTaskAccretionExplanationUI.activeSelf)
        {
            StartCoroutine(HideUI(phase3NearTaskAccretionExplanationUI));
            StartCoroutine(HideUI(nextButtonUI));
            environmentAnimationController.AnimateShowObjects(phase3NearEnvironment);
            phase3NearDepthCueTogglerPanel.SetStartingCueState();
            phase3NearDepthCueTogglerPanel.EnableAllCues();
        }
        else if (phase3NearDepthCueTogglerPanelUI.activeSelf && phase3NearDepthCueTogglerPanelUI.transform.localScale == Vector3.one)
        {
            phase3NearDepthCueTogglerPanel.DisableActiveToggle();
            StartCoroutine(HideUIButKeepActive(phase3NearDepthCueTogglerPanelUI));
            StartCoroutine(HideUI(nextButtonUI));
            StartCoroutine(HideUI(phase3NearHidePanelButtonUI));
        }
        else if (phase3NearTaskUI.activeSelf)
        {
            // All toggles gone -> Start next task and reset panel
            if (phase3NearDepthCueTogglerPanel.AllTogglesDeactivated())
            {
                environmentAnimationController.AnimateHideObjects(phase3NearEnvironment);
                phase3NearDepthCueTogglerPanel.ResetTogglesAndCues();
                phase3NearDepthCueTogglerPanel.RestoreNormalView();
                phase3NearDepthCueTogglerPanelUI.SetActive(false);
                StartCoroutine(HideUI(nextButtonUI));
                StartCoroutine(ShowUI(finalUI));

                // save results
                nearTaskDataManager.SaveCSV();
            }
            else
            {
                StartCoroutine(ShowUI(phase3NearDepthCueTogglerPanelUI));
            }
        }
        //Debug.Log("LENKUEP" + Application.persistentDataPath);
    }
}
