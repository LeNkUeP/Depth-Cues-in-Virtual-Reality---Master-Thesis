using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RelativePositionTask : Task
{
    [Header("UI")]
    public GameObject taskUI;
    public GameObject finishUI;
    public GameObject successUI;
    public GameObject failureUI;

    [Header("Toggles")]
    public Toggle blueToggle;
    public Toggle redToggle;
    public Toggle yellowToggle;
    public Toggle greenToggle;
    public Toggle dontKnowToggle;

    [Header("Cubes")]
    public GameObject blueCube;
    public GameObject redCube;
    public GameObject yellowCube;
    public GameObject greenCube;

    [Header("Humans")]
    public GameObject blueCubeHuman;
    public GameObject redCubeHuman;
    public GameObject yellowCubeHuman;
    public GameObject greenCubeHuman;

    [Header("Spawn Positions")]
    public GameObject[] spawnPositions;

    public CueFarTaskDataManager cueFarTaskDataManager;

    private GameObject[] cubes;
    private GameObject[] humans;
    private Toggle[] colorToggles;

    private void Awake()
    {
        cubes = new GameObject[] { blueCube, redCube, yellowCube, greenCube };
        humans = new GameObject[] { blueCubeHuman, redCubeHuman, yellowCubeHuman, greenCubeHuman };

        colorToggles = new Toggle[]
        {
            blueToggle,
            redToggle,
            yellowToggle,
            greenToggle
        };
    }

    public override void ProcessTaskCompletion()
    {
        if (dontKnowToggle.isOn)
        {
            StartCoroutine(HideEnterButton());
            ResetToggleGroup();
            RandomizeScene();

            if (!trainingMode)
            {
                cueFarTaskDataManager.AddTaskResult("nicht eindeutig");
                phaseManager.Next();
            }
            return;
        }

        float minDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < cubes.Length; i++)
        {
            float distance = Vector3.Distance(
                cubes[i].transform.position,
                Camera.main.transform.position
            );

            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        bool success = colorToggles[closestIndex].isOn;
        if (!trainingMode)
        {
            if (success)
            {
                cueFarTaskDataManager.AddTaskResult("korrekt");
            }
            else
            {
                cueFarTaskDataManager.AddTaskResult("falsch");
            }
        }

        SetFinishUI(success);

        StartCoroutine(HideEnterButton());
        StartCoroutine(TriggerUISwitch());
    }

    IEnumerator TriggerUISwitch()
    {
        yield return new WaitForSeconds(2f);

        StartCoroutine(HideFinishedUIButton());

        ResetToggleGroup();
        RandomizeScene();

        if (!trainingMode)
        {
            phaseManager.Next();
        }
    }

    void SetFinishUI(bool success)
    {
        finishUI.SetActive(true);

        successUI.SetActive(success);
        failureUI.SetActive(!success);
    }

    public void ResetToggleGroup()
    {
        foreach (Toggle t in colorToggles)
        {
            t.isOn = false;
        }

        dontKnowToggle.isOn = false;
    }

    public override void RandomizeScene()
    {
        iterationCounter++;
        if (iterationCounter >= amountOfTrainingIterations)
        {
            trainingMode = false;
        }

        for (int i = 0; i < humans.Length; i++)
        {
            humans[i].transform.SetParent(cubes[i].transform);
        }

        GameObject[] shuffled = (GameObject[])spawnPositions.Clone();
        for (int i = 0; i < shuffled.Length; i++)
        {
            int rand = Random.Range(i, shuffled.Length);

            GameObject temp = shuffled[i];
            shuffled[i] = shuffled[rand];
            shuffled[rand] = temp;
        }

        for (int i = 0; i < cubes.Length; i++)
        {
            // hide for short amount, height in field of view causes jump otherwise
            cubes[i].GetComponentInChildren<Renderer>().enabled = false;
            humans[i].GetComponentInChildren<Renderer>().enabled = false;

            cubes[i].transform.position = shuffled[i].transform.position;
            cubes[i].transform.rotation = Random.rotation;
        }

        for (int i = 0; i < humans.Length; i++)
        {
            humans[i].transform.SetParent(transform);
        }

        StartCoroutine(ReEnableObjectRenderers());
    }

    private IEnumerator ReEnableObjectRenderers()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].GetComponentInChildren<Renderer>().enabled = true;
            humans[i].GetComponentInChildren<Renderer>().enabled = true;
        }
    }

    public void OnToggleChanged()
    {
        if (AnyToggleSelected())
        {
            StartCoroutine(ShowEnterButton());
        }
        else
        {
            StartCoroutine(HideEnterButton());
        }
    }

    bool AnyToggleSelected()
    {
        if (dontKnowToggle.isOn)
            return true;

        foreach (Toggle t in colorToggles)
        {
            if (t.isOn)
                return true;
        }

        return false;
    }

    IEnumerator ShowEnterButton()
    {
        enterButtonUI.SetActive(true);
        enterButtonUI.GetComponent<Animator>().SetTrigger("show");
        yield return null;
    }

    IEnumerator HideEnterButton()
    {
        enterButtonUI.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(.5f);
        enterButtonUI.SetActive(false);
    }

    IEnumerator HideFinishedUIButton()
    {
        finishUI.GetComponent<Animator>().SetTrigger("hide");

        yield return new WaitForSeconds(.5f);

        finishUI.SetActive(false);
        successUI.SetActive(false);
        failureUI.SetActive(false);
    }
}