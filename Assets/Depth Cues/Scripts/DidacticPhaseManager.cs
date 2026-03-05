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
    public GameObject phase1IntroUI;
    public GameObject didacticUI;
    public GameObject phase2IntroUI;
    public GameObject additionalRatingTaskUI;
    public GameObject toggleDepthCueUI;
    public GameObject hideButtonUI;
    public GameObject phase3IntroUI;

    [Header("Managers & Controllers")]
    public EnvironmentAnimationController environmentAnimationController;
    public StudyDataManager studyDataManager;

    [Header("Scene Environments")]
    public GameObject phase2Environment;

    private void Start()
    {
        DisableAllUI();
        // hide phase 2 scene
        environmentAnimationController.DisableAllObjects(phase2Environment);

        PreGameInitializations();

        // first only start button should be visible to start prototype
        startButtonUI.SetActive(true);
        startButtonUI.GetComponent<Animator>().SetTrigger("pulse");
    }

    private void DisableAllUI()
    {
        startButtonUI.SetActive(false);
        welcomeUI.SetActive(false);
        nextButtonUI.SetActive(false);
        explanationDepthCuesUI.SetActive(false);
        explanationBinoAndMonoUI.SetActive(false);
        explanationDidacticPartUI.SetActive(false);
        phase1IntroUI.SetActive(false);
        didacticUI.SetActive(false);
        phase2IntroUI.SetActive(false);
        additionalRatingTaskUI.SetActive(false);
        toggleDepthCueUI.SetActive(false);
        hideButtonUI.SetActive(false);
        phase3IntroUI.SetActive(false);
    }

    private void PreGameInitializations()
    {
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
            StartCoroutine(SwitchUI(explanationDidacticPartUI, phase1IntroUI));
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
            //studyDataManager.SaveCSV();
            //Debug.Log("LENKUEP" + Application.persistentDataPath);
        }

    }
}
