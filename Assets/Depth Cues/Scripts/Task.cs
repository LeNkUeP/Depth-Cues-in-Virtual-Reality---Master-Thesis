using System.Collections;
using UnityEngine;

public abstract class Task : MonoBehaviour
{
    public DidacticPhaseManager phaseManager;
    public GameObject enterButtonUI;
    public bool trainingMode;
    public int amountOfTrainingIterations;
    public int iterationCounter = 1;

    public abstract void ProcessTaskCompletion();

    public abstract void RandomizeScene();
}
