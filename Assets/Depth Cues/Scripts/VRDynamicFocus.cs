using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VRDynamicFocus : MonoBehaviour
{
    [Header("Cast Settings")]
    public float maxDistance = 20f;
    public float sphereRadius = 0.1f; // Neuer Parameter für SphereCast
    public LayerMask focusLayer;

    [Header("Focus Settings")]
    public float focusSpeed = 5f;
    public float defaultFocusDistance = 10f;

    [Header("References")]
    public Transform cameraTransform;
    public Volume postProcessVolume;

    private DepthOfField depthOfField;
    private float targetFocusDistance;
    private float currentFocusDistance;

    void Start()
    {
        if (postProcessVolume.profile.TryGet(out depthOfField))
        {
            depthOfField.active = true;
        }

        currentFocusDistance = defaultFocusDistance;
        targetFocusDistance = defaultFocusDistance;
    }

    void Update()
    {
        HandleSphereCast();
        UpdateFocus();
    }

    void HandleSphereCast()
    {
        RaycastHit hit;
        Vector3 origin = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;

        // SphereCast statt Raycast
        if (Physics.SphereCast(origin, sphereRadius, direction, out hit, maxDistance, focusLayer))
        {
            targetFocusDistance = hit.distance;
        }
        else
        {
            targetFocusDistance = defaultFocusDistance;
        }
    }

    void UpdateFocus()
    {
        currentFocusDistance = Mathf.Lerp(
            currentFocusDistance,
            targetFocusDistance,
            Time.deltaTime * focusSpeed
        );

        if (depthOfField != null)
        {
            depthOfField.focusDistance.value = currentFocusDistance;
        }
    }
}