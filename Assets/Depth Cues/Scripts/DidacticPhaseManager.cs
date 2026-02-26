using Oculus.Interaction;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class DidacticPhaseManager: MonoBehaviour
{
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
    public EnvironmentAnimationController environmentAnimationController;

    private Vector3 toggleMenuStarterPosition;
    private Renderer[] allRenderers;
    private OVRManager ovrManager;

    private void Start()
    {
        PreGameInitializations();

        DisableAllUI();
        startButtonUI.SetActive(true);
        startButtonUI.GetComponent<Animator>().SetTrigger("pulse");
    }

    private void DisableAllUI()
    {
        welcomeUI.SetActive(false);
        nextButtonUI.SetActive(false);
        explanationDepthCuesUI.SetActive(false);
        explanationBinoAndMonoUI.SetActive(false);
        explanationDidacticPartUI.SetActive(false);
        //toggleDepthCueUI.SetActive(false);
    }

    private void PreGameInitializations()
    {
        //ovrManager = FindFirstObjectByType<OVRManager>();
        //ovrManager.headPoseRelativeOffsetTranslation = new Vector3(0,0.5f,0);

        //allRenderers = FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().CreateBlurClonesForAffectedObjects();

        toggleDepthCueUI.SetActive(true);
        toggleMenuStarterPosition = toggleDepthCueUI.transform.position;
        toggleDepthCueUI.transform.position = Vector3.one * 1000;
        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleAtmosphericPerspective();
        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleAtmosphericPerspective();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleShapeFromShading();
        //toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ToggleShapeFromShading();

        // keep active to preload videos and prevent lag
        didacticUI.SetActive(true);
        didacticUI.GetComponent<PokeInteractable>().enabled = false;
        didacticUI.GetComponentInChildren<Canvas>().enabled = false;

        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().HideCurrentRatingSlider();
        toggleDepthCueUI.GetComponent<ToggleDepthCuePanel>().ResetWasToggledState();
    }

    private void ShowKeyboard()
    {
        //TouchScreenKeyboard overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    private IEnumerator StartPrototype()
    {
        startButtonUI.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(.5f);
        startButtonUI.SetActive(false);
        welcomeUI.SetActive(true);
        welcomeUI.GetComponent<Animator>().SetTrigger("show");
        nextButtonUI.SetActive(true);
        nextButtonUI.GetComponent<Animator>().SetTrigger("show");
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
        StartCoroutine(StartPrototype());
    }

    public void Next()
    {
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
            toggleDepthCueUI.SetActive(false);
            toggleDepthCueUI.transform.position = toggleMenuStarterPosition;
            StartCoroutine(HideUI(nextButtonUI));
            StartCoroutine(SwitchUI(additionalRatingTaskUI, toggleDepthCueUI));
            StartCoroutine(DelayedShowUI(hideButtonUI));
            environmentAnimationController.EnableAllChildren();
        }
        else if (toggleDepthCueUI.activeSelf)
        {
            StartCoroutine(HideUI(hideButtonUI));
            StartCoroutine(SwitchUI(toggleDepthCueUI, phase3IntroUI));
        }
    }
}
