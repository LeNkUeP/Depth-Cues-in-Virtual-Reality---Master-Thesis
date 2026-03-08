using Oculus.Interaction;
using UnityEngine;

public class GhostContactDetector : MonoBehaviour
{
    public PrecisePositioningTask task;
    public GameObject grabObject;

    private void OnEnable()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == grabObject)
        {
            StartCoroutine(task.ShowEnterButton());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == grabObject)
        {
            StartCoroutine(task.HideEnterButton());
        }
    }
}
