using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnvironmentAnimationController : MonoBehaviour
{
    [Header("Referenzen")]
    public Transform ground; // Boden Referenz

    [Header("Zeit Einstellungen")]
    public float minDuration = 0.5f;
    public float maxDuration = 2f;

    [Header("Start Verzögerung")]
    public float minDelay = 0f;
    public float maxDelay = 1.5f;

    [Header("Bewegungs Multiplikator (abhängig von Collider Höhe)")]
    public float minHeightMultiplier = 1.0f;
    public float maxHeightMultiplier = 2.0f;

    // 🔹 Speichert ursprüngliche Scales
    private Dictionary<Transform, Vector3> originalScales = new Dictionary<Transform, Vector3>();
    private void Awake()
    {
        // Original-Scale aller direkten Children speichern
        foreach (Transform child in transform)
        {
            originalScales[child] = child.localScale;
        }
    }

    private void Start()
    {
        DisableAllChildren();
    }

    public void DisableAllChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void EnableAllChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void AnimateShowChildren()
    {
        foreach (Transform child in transform)
        {
            StartCoroutine(AnimateChild(child));
        }
    }

    private IEnumerator AnimateChild(Transform child)
    {
        Collider col = child.GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning(child.name + " hat keinen Collider!");
            yield break;
        }

        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        float duration = Random.Range(minDuration, maxDuration);

        float objectHeight = col.bounds.size.y;

        // 🔹 Ursprüngliche Zielposition merken
        Vector3 targetPosition = child.position;

        // 🔹 Zufällige Strecke abhängig von Höhe
        float heightMultiplier = Random.Range(minHeightMultiplier, maxHeightMultiplier);
        float travelDistance = objectHeight * heightMultiplier;

        float groundY = ground.position.y;

        // 🔹 Startposition unter dem Boden
        Vector3 startPosition = new Vector3(
            targetPosition.x,
            groundY - travelDistance,
            targetPosition.z
        );

        // Objekt nach unten setzen
        child.position = startPosition;
        child.gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            t = Mathf.SmoothStep(0, 1, t);

            child.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        child.position = targetPosition;
    }

    public void AnimateShowChildrenScale()
    {
        foreach (Transform child in transform)
        {
            StartCoroutine(AnimateChildScale(child));
        }
    }

    private IEnumerator AnimateChildScale(Transform child)
    {
        if (!originalScales.ContainsKey(child))
            yield break;

        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        float duration = Random.Range(minDuration, maxDuration);

        Vector3 targetScale = originalScales[child];

        child.localScale = Vector3.zero;
        child.gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            child.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        child.localScale = targetScale;
    }
}
