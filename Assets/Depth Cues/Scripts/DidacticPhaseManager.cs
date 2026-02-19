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
    public GameObject didacticUI;
    public GameObject toggleDepthCueUI;

    private void Start()
    {
        // to preload videos and prevent lag
        didacticUI.SetActive(true);
        DisableAllUI();
        startButtonUI.GetComponent<Animator>().SetTrigger("pulse");
    }

    private void DisableAllUI()
    {
        welcomeUI.SetActive(false);
        nextButtonUI.SetActive(false);
        explanationDepthCuesUI.SetActive(false);
        explanationBinoAndMonoUI.SetActive(false);
        explanationDidacticPartUI.SetActive(false);
        didacticUI.SetActive(false);
        //toggleDepthCueUI.SetActive(false);
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
            nextButtonUI.GetComponent<Animator>().SetTrigger("hide");
            StartCoroutine(SwitchUI(explanationDidacticPartUI, didacticUI));
        }
    }
}
