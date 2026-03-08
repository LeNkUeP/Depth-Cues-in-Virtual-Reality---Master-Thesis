using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DepthCueChoicePanel : MonoBehaviour
{
    [Header("Toggle Group")]
    public ToggleGroup depthCueGroup;

    public CueIdentificationDataManager identificationDataManager;
    public RandomDepthCueDisabler depthCueDisabler;
    public DidacticPhaseManager didacticPhaseManager;

    public Toggle dontKnowToggle;

    [Header("UI")]
    public GameObject enterButtonUI;

    private string currentDisabledCue;

    private void OnEnable()
    {
        depthCueDisabler.InitCues();
        DeactivateNextCue();
    }

    public void OnChoiceMade()
    {
        Toggle selectedToggle = depthCueGroup.GetFirstActiveToggle();
        string selectedCue = selectedToggle != null ? selectedToggle.name : "DontKnow";

        identificationDataManager.SetChoice(selectedCue);

        ResetToggles();

        if (depthCueDisabler.HasRemainingCues())
            DeactivateNextCue();
        else
            didacticPhaseManager.Next();
    }

    private void DeactivateNextCue()
    {
        // enable again, only one
        depthCueDisabler.depthCueToggler.EnableAllCues();
        currentDisabledCue = depthCueDisabler.DisableRandomDepthCue();
    }

    public void OnToggleChanged()
    {
        if (depthCueGroup.AnyTogglesOn())
        {
            StartCoroutine(ShowEnterButton());
        }
        else
        {
            StartCoroutine(HideNextButton());
        }
    }

    public void ResetToggles()
    {
        foreach (var toggle in depthCueGroup.ActiveToggles())
        {
            toggle.isOn = false;
        }
    }

    public Toggle GetSelectedToggle()
    {
        return depthCueGroup.GetFirstActiveToggle();
    }

    public string GetSelectedDepthCue()
    {
        Toggle selected = depthCueGroup.GetFirstActiveToggle();

        if (selected != null)
            return selected.name;

        return "DontKnow";
    }

    IEnumerator ShowEnterButton()
    {
        enterButtonUI.SetActive(true);
        enterButtonUI.GetComponent<Animator>().SetTrigger("show");
        yield return null;
    }

    IEnumerator HideNextButton()
    {
        enterButtonUI.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(.5f);
        enterButtonUI.SetActive(false);
    }
}