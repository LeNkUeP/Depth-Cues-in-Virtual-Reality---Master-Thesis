using Oculus.Interaction.Samples;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnvironmentAnimationController : MonoBehaviour
{
    [Header("General")]
    public Transform ground;

    [Header("Duration")]
    public float minDuration = 1.5f;
    public float maxDuration = 3f;

    [Header("Delay")]
    public float minDelay = 0f;
    public float maxDelay = 0.5f;

    [Header("Distance multiplier")]
    public float minHeightMultiplier = 1.5f;
    public float maxHeightMultiplier = 1.6f;

    public void DisableAllObjects(GameObject sceneEnvironment)
    {
        foreach (Transform sceneObject in sceneEnvironment.transform)
        {
            sceneObject.gameObject.SetActive(false);

            if (sceneObject.GetComponentInChildren<Rigidbody>() != null)
                sceneObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
            if (sceneObject.GetComponentInChildren<RespawnOnDrop>() != null)
                sceneObject.GetComponentInChildren<RespawnOnDrop>().enabled = false;
        }
    }

    public void DisableAllObjects(GameObject[] sceneEnvironment)
    {
        foreach (GameObject sceneObject in sceneEnvironment)
        {
            sceneObject.gameObject.SetActive(false);

            if (sceneObject.GetComponentInChildren<Rigidbody>() != null)
                sceneObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
            if (sceneObject.GetComponentInChildren<RespawnOnDrop>() != null)
                sceneObject.GetComponentInChildren<RespawnOnDrop>().enabled = false;
        }
    }

    public void EnableAllObjects(GameObject sceneEnvironment)
    {
        foreach (Transform sceneObject in sceneEnvironment.transform)
        {
            sceneObject.gameObject.SetActive(true);
        }
    }

    public void AnimateShowObjects(GameObject sceneEnvironment)
    {
        foreach (Transform sceneObject in sceneEnvironment.transform)
        {
            StartCoroutine(AnimateShowObject(sceneObject));
        }
    }

    public void AnimateShowObjects(GameObject[] sceneEnvironment)
    {
        foreach (GameObject sceneObject in sceneEnvironment)
        {
            StartCoroutine(AnimateShowObject(sceneObject.transform));
        }
    }

    private IEnumerator AnimateShowObject(Transform sceneObject)
    {
        Renderer renderer = sceneObject.GetComponent<Renderer>();

        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        float duration = Random.Range(minDuration, maxDuration);

        // if no height is measurable, use default height, animation may look weird but is working
        float objectHeight = 5f;
        if (renderer != null)
        {
            objectHeight = renderer.bounds.size.y;
        }

        // save target position
        Vector3 targetPosition = sceneObject.position;

        float groundY = ground.position.y;

        // random distance, minimal = objectheight + y-coord, scale with multipliers 
        float heightMultiplier = Random.Range(minHeightMultiplier, maxHeightMultiplier);
        float travelDistance = objectHeight * heightMultiplier + Mathf.Abs(groundY-targetPosition.y);


        // animate from this hidden position (under ground)
        Vector3 startFromPosition = new Vector3(
            targetPosition.x,
            groundY - travelDistance,
            targetPosition.z
        );

        // move object under ground
        sceneObject.position = startFromPosition;
        sceneObject.gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            t = Mathf.SmoothStep(0, 1, t);

            sceneObject.position = Vector3.Lerp(startFromPosition, targetPosition, t);
            yield return null;
        }

        // reset important physic components
        sceneObject.position = targetPosition;
        if (sceneObject.GetComponentInChildren<Rigidbody>() != null)
            sceneObject.GetComponentInChildren<Rigidbody>().isKinematic = false;

        if (sceneObject.GetComponentInChildren<RespawnOnDrop>() != null)
            sceneObject.GetComponentInChildren<RespawnOnDrop>().enabled = true;
    }

    public void AnimateHideObjects(GameObject sceneEnvironment)
    {
        foreach (Transform sceneObject in sceneEnvironment.transform)
        {
            StartCoroutine(AnimateHideObject(sceneObject));
        }
    }

    public void AnimateHideObjects(GameObject[] sceneEnvironment)
    {
        foreach (GameObject sceneObject in sceneEnvironment)
        {
            StartCoroutine(AnimateHideObject(sceneObject.transform));
        }
    }

    private IEnumerator AnimateHideObject(Transform sceneObject)
    {
        Renderer renderer = sceneObject.GetComponent<Renderer>();

        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        float duration = Random.Range(minDuration, maxDuration);

        // if no height is measurable, use default height, animation may look weird but is working
        float objectHeight = 5f;
        if (renderer != null)
        {
            objectHeight = renderer.bounds.size.y;
        }

        // save target position
        Vector3 startFromPosition = sceneObject.position;

        float groundY = ground.position.y;

        // random distance, minimal = objectheight + y-coord, scale with multipliers 
        float heightMultiplier = Random.Range(minHeightMultiplier, maxHeightMultiplier);
        float travelDistance = objectHeight * heightMultiplier + Mathf.Abs(groundY - startFromPosition.y);

        // animate from this hidden position (under ground)
        Vector3 targetPosition = new Vector3(
            startFromPosition.x,
            groundY - travelDistance,
            startFromPosition.z
        );

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            t = Mathf.SmoothStep(0, 1, t);

            sceneObject.position = Vector3.Lerp(startFromPosition, targetPosition, t);
            yield return null;
        }

        sceneObject.gameObject.SetActive(false);
    }
}
