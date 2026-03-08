using Oculus.Interaction;
using UnityEngine;

public class Phase3Interactableobject : MonoBehaviour
{
    public GameObject toggleUI;
    public PrecisePositioningTask task;

    private Grabbable grabbable;
    private bool firstGrabTriggered = false;

    void Start()
    {
        grabbable = GetComponent<Grabbable>();

        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised += OnPointerEvent;
        }
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select && !firstGrabTriggered)
        {
            firstGrabTriggered = true;

            if (task != null)
            {
                task.StartTimer();
            }
        }
    }

    private bool isToggleUIVisible()
    {
        return toggleUI.transform.localScale == Vector3.one;
    }

    private void Update()
    {
        if (isToggleUIVisible())
        {
            GetComponent<TouchHandGrabInteractable>().enabled = false;
            GetComponent<Grabbable>().enabled = false;
        }
        else
        {
            GetComponent<TouchHandGrabInteractable>().enabled = true;
            GetComponent<Grabbable>().enabled = true;
        }
    }

    private void OnDestroy()
    {
        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised -= OnPointerEvent;
        }
    }

    public void ResetGrabState()
    {
        firstGrabTriggered = false;
    }
}