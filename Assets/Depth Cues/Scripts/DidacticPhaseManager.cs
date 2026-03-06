using Oculus.Interaction;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class DidacticPhaseManager: MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject startButtonUI;
    public GameObject welcomeUI;
    public GameObject nextButtonUI;
    public GameObject explanationDepthCuesUI;
    public GameObject explanationBinoAndMonoUI;
    public GameObject explanationDidacticPartUI;
    public GameObject explanationTasksUI;
    public GameObject phase1IntroUI;
    public GameObject didacticUI;
    public GameObject phase2IntroUI;
    public GameObject additionalRatingTaskUI;
    public GameObject toggleDepthCueUI;
    public GameObject hideButtonUI;
    public GameObject phase3IntroUI;
    public GameObject phase3FarTaskUI;

    [Header("Managers & Controllers")]
    public EnvironmentAnimationController environmentAnimationController;
    public ToggleDepthCueController toggleDepthCueController;
    public StudyDataManager studyDataManager;

    [Header("Scene Environments")]
    public GameObject phase2Environment;
    public GameObject phase3Environment;

    private void Start()
    {
        EnableAllEnvironments();
        PreGameInitializations();
    }

    public void EnableAllEnvironments()
    {
        phase2Environment.SetActive(true);
        phase3Environment.SetActive(true);
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
        didacticUI.SetActive(false);
        phase2IntroUI.SetActive(false);
        additionalRatingTaskUI.SetActive(false);
        toggleDepthCueUI.SetActive(false);
        hideButtonUI.SetActive(false);
        phase3IntroUI.SetActive(false);
        phase3FarTaskUI.SetActive(false);
    }

    private void PreGameInitializations()
    {
        DisableAllUI();
        // hide phase 2 scene
        environmentAnimationController.DisableAllObjects(phase2Environment);
        environmentAnimationController.DisableAllObjects(phase3Environment);

        // Must be active to complete initialization process
        toggleDepthCueUI.SetActive(true);
        // only hide visually
        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleVisibility();
        // Switch back visibility icon because this is a special case at the beginning
        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().SwitchHideUIEyeIcons();

        // Prevents lag/initialization in moment of usage
        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleAtmosphericPerspective();
        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleAtmosphericPerspective();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleLinearPerspective();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleLinearPerspective();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleShapeFromShading();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleShapeFromShading();

        // keep active to preload videos and prevent lag
        didacticUI.SetActive(true);
        didacticUI.GetComponent<DidacticDepthCuePanel>().ToggleVisibility();

        // Toggle depth cue panel slider should be invisible when no cue was selected
        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().HideCurrentRatingSlider();
        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ResetWasToggledState();

        // first only start button should be visible to start prototype
        startButtonUI.SetActive(true);
        startButtonUI.GetComponent<Animator>().SetTrigger("pulse");
    }

    private void ShowKeyboard()
    {
        //TouchScreenKeyboard overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    private IEnumerator SwitchUI(GameObject old, GameObject next)
    {
        old.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(.5f);
        old.SetActive(false);
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
            didacticUI.GetComponentInChildren<Canvas>().enabled = true;
            didacticUI.GetComponent<PokeInteractable>().enabled = true;
            didacticUI.SetActive(false);

            StartCoroutine(HideUI(nextButtonUI));
            StartCoroutine(SwitchUI(phase1IntroUI, didacticUI));
        }
        //PHASE 2: DEPTH CUE EFFECTS
        else if (didacticUI.activeSelf)
        {
            didacticUI.GetComponent<DidacticDepthCuePanel>().HideTextAndVideoUI();

            StartCoroutine(SwitchUI(didacticUI, phase2IntroUI));
        }
        else if (phase2IntroUI.activeSelf)
        {
            StartCoroutine(SwitchUI(phase2IntroUI, additionalRatingTaskUI));
        }
        else if (additionalRatingTaskUI.activeSelf)
        {
            StartCoroutine(HideUI(nextButtonUI));
            StartCoroutine(SwitchUI(additionalRatingTaskUI, toggleDepthCueUI));
            toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().SetStartingCueState();
            StartCoroutine(DelayedShowUI(hideButtonUI));

            // show scene objects
            environmentAnimationController.AnimateShowObjects(phase2Environment);
        }
        // PHASE 3: EXPLORATIVE TASKS
        else if (toggleDepthCueUI.activeSelf)
        {
            // Unnatural depth cues need to be resetted/deactivated
            toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().RestoreNormalView();

            environmentAnimationController.AnimateHideObjects(phase2Environment);
            StartCoroutine(HideUI(hideButtonUI));
            StartCoroutine(SwitchUI(toggleDepthCueUI, phase3IntroUI));
        }
        else if (phase3IntroUI.activeSelf)
        {
            StartCoroutine(SwitchUI(phase3IntroUI, phase3FarTaskUI));
            StartCoroutine(HideUI(nextButtonUI));
            environmentAnimationController.AnimateShowObjects(phase3Environment);
            toggleDepthCueController.SetStartingCueState();
            //studyDataManager.SaveCSV();
            //Debug.Log("LENKUEP" + Application.persistentDataPath);
            StartCoroutine(TEst());
        }

    }

    private IEnumerator TEst()
    {
        yield return new WaitForSeconds(5f);
        toggleDepthCueController.DisableAllCues();
        yield return new WaitForSeconds(3f);
        toggleDepthCueController.ToggleLinearPerspective();
        yield return new WaitForSeconds(3f);
        toggleDepthCueController.ToggleHeightInFieldOfView();
        yield return new WaitForSeconds(3f);
        toggleDepthCueController.ToggleKnownSize();
        yield return new WaitForSeconds(3f);
        toggleDepthCueController.ToggleRelativeSize();
        yield return new WaitForSeconds(3f);
        toggleDepthCueController.ToggleMotionParallax();
        yield return new WaitForSeconds(3f);
        toggleDepthCueController.ToggleShapeFromShading();
        yield return new WaitForSeconds(3f);
        toggleDepthCueController.ToggleOcclusion();
    }
}
