using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrecisePositioningTask : Task
{
    [Header("Scene Objects")]
    public GameObject[] ghostObjects;
    public GameObject[] grabObjects;

    [Header("UI Objects")]
    public GameObject finishUI;
    public TextMeshProUGUI timeUI;
    public TextMeshProUGUI percentageUI;

    [Header("Spawn Points")]
    public GameObject[] ghostSpawns;
    public GameObject[] grabObjectSpawns;

    [Header("Materials")]
    public Material transparentMaterial;

    public GameObject targetInstance;
    public GameObject interactableInstance;

    public CueNearTaskDataManager cueNearTaskDataManager;

    private float startTime;

    public override void RandomizeScene()
    {
        iterationCounter++;
        if (iterationCounter >= amountOfTrainingIterations)
        {
            trainingMode = false;
        }

        HideAllObjects();

        int randomIndex = Random.Range(0, ghostObjects.Length);

        targetInstance = ghostObjects[randomIndex];
        interactableInstance = grabObjects[randomIndex];

        Transform ghostSpawn = ghostSpawns[Random.Range(0, ghostSpawns.Length)].transform;
        Transform grabSpawn = grabObjectSpawns[Random.Range(0, grabObjectSpawns.Length)].transform;

        Quaternion ghostRotation = Random.rotation;

        float randomY = Random.Range(0f, 360f);
        Quaternion grabRotation = Quaternion.Euler(0f, randomY, 0f);

        targetInstance.transform.SetPositionAndRotation(ghostSpawn.position, ghostRotation);
        interactableInstance.transform.SetPositionAndRotation(grabSpawn.position, grabRotation);

        targetInstance.SetActive(true);
        interactableInstance.SetActive(true);

        interactableInstance.GetComponent<Phase3Interactableobject>().ResetGrabState();

        //ApplyTransparentMaterial(targetInstance);
        //MakeGhostStatic(targetInstance);
    }

    public override void ProcessTaskCompletion()
    {
        float duration = Time.time - startTime;

        float positionError = Vector3.Distance(
            interactableInstance.transform.position,
            targetInstance.transform.position
        );

        float rotationError = Quaternion.Angle(
            interactableInstance.transform.rotation,
            targetInstance.transform.rotation
        );

        float maxPositionError = 0.25f;
        float maxRotationError = 180f;

        // normalized error function, final score
        float positionScore = Mathf.Clamp01(1f - (positionError / maxPositionError));
        float rotationScore = Mathf.Clamp01(1f - (rotationError / maxRotationError));

        float finalScore = (positionScore * 0.6f + rotationScore * 0.4f) * 100f;

        if (!trainingMode)
        {
            cueNearTaskDataManager.AddTaskResult(duration, finalScore/100f);
        }

        SetFinishUI(duration, finalScore);

        StartCoroutine(HideButStayActiveEnterButton());
        StartCoroutine(TriggerUISwitch());
    }

    IEnumerator TriggerUISwitch()
    {
        yield return new WaitForSeconds(2f);

        //StartCoroutine(HideFinishedUIButton());
        finishUI.SetActive(false);

        RandomizeScene();

        if (!trainingMode)
        {
            phaseManager.Next();
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
    }

    void SetFinishUI(float duration, float percent)
    {
        finishUI.SetActive(true);
        timeUI.text = "" + duration + " Sekunden";
        percentageUI.text = "" + percent + " %";
    }

    IEnumerator HideFinishedUIButton()
    {
        finishUI.GetComponent<Animator>().SetTrigger("hide");

        yield return new WaitForSeconds(.5f);

        finishUI.SetActive(false);
    }

    void HideAllObjects()
    {
        foreach (GameObject obj in ghostObjects)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in grabObjects)
        {
            obj.SetActive(false);
        }
    }

    public IEnumerator ShowEnterButton()
    {
        enterButtonUI.SetActive(true);
        enterButtonUI.GetComponent<Animator>().SetTrigger("show");
        yield return null;
    }

    public IEnumerator HideEnterButton()
    {
        enterButtonUI.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(.5f);
        enterButtonUI.SetActive(false);
    }

    public IEnumerator HideButStayActiveEnterButton()
    {
        enterButtonUI.GetComponent<Animator>().SetTrigger("hide");
        yield return null;
    }

    void ApplyTransparentMaterial(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            r.material = transparentMaterial;
        }
    }

    void MakeGhostStatic(GameObject obj)
    {
        foreach (var comp in obj.GetComponentsInChildren<TouchHandGrabInteractable>())
            Destroy(comp);

        foreach (var comp in obj.GetComponentsInChildren<Grabbable>())
            Destroy(comp);

        foreach (var col in obj.GetComponentsInChildren<Collider>())
            Destroy(col);

        BoxCollider trigger = obj.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
    }
}