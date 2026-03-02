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

    [Header("Scene Objects")]
    public GameObject phase2Environment;
    public GameObject[] phase2EnvironmentObjects;

    private Vector3 toggleMenuStarterPosition;

    private void Start()
    {
        DisableAllUI();
        //environmentAnimationController.DisableAllObjects(phase2Environment);

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
        // Must be active to complete initialization process, hide by translating far away 
        toggleDepthCueUI.SetActive(true);
        toggleMenuStarterPosition = toggleDepthCueUI.transform.position;
        toggleDepthCueUI.transform.position = Vector3.one * 1000;

        // Prevents lag/initialization in moment of usage
        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleAtmosphericPerspective();
        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleAtmosphericPerspective();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleShapeFromShading();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleShapeFromShading();

        // keep active to preload videos and prevent lag
        didacticUI.SetActive(true);
        didacticUI.GetComponent<PokeInteractable>().enabled = false;
        didacticUI.GetComponentInChildren<Canvas>().enabled = false;

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
            // reset depth cue panels position and proceed with normal show animation
            toggleDepthCueUI.SetActive(false);
            toggleDepthCueUI.transform.position = toggleMenuStarterPosition;

            StartCoroutine(HideUI(nextButtonUI));
            StartCoroutine(SwitchUI(additionalRatingTaskUI, toggleDepthCueUI));
            StartCoroutine(DelayedShowUI(hideButtonUI));

            // show scene objects
            environmentAnimationController.AnimateShowObjects(phase2EnvironmentObjects);
        }
        // PHASE 3: EXPLORATIVE TASKS
        else if (toggleDepthCueUI.activeSelf)
        {
            // Unnatural depth cues need to be resetted/deactivated
            toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().RestoreNormalView();

            environmentAnimationController.AnimateHideObjects(phase2EnvironmentObjects);
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
