using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DynamicDOF : MonoBehaviour
{
    [Header("References")]
    public Volume globalVolume;
    public Camera playerCamera;

    [Header("Focus Settings")]
    public float maxFocusDistance = 1000f;   // quasi unendlich
    public float nearSoftDistance = 2f;      // bis hierhin abnehmende Unschaerfe

    [Header("Bokeh Settings")]
    [Range(1, 32)]
    public float maxAperture = 32f; // sehr unscharf nah
    [Range(1, 32)]
    public float minAperture = 8f;   // leicht unscharf fern
    [Range(1, 300)]
    public float focalLength = 50f;  // Brennweite
    [Range(3, 9)]
    public int bladeCount = 6;       // Bokeh-Form

    [Header("Smoothing")]
    public float focusSmoothTime = 0.1f; // Zeit, um auf Ziel zu kommen

    [Header("Cone Tolerance")]
    [Range(1f, 30f)]
    public float coneAngle = 10f;   // Blicktoleranz in Grad
    public LayerMask focusLayer;    // Welche Layer sind fokussierbar

    [Header("Raycast Visualization")]
    public float rayDownwardAngle = 10f; // Ray leicht nach unten

    private DepthOfField dof;
    private float targetFocusDistance;
    private float currentFocusDistance;
    private float focusVelocity = 0f; // für SmoothDamp

    private LineRenderer rayVisualizer;

    void Start()
    {
        if (!globalVolume.profile.TryGet(out dof))
        {
            Debug.LogError("DepthOfField not found in Volume!");
            enabled = false;
            return;
        }

        // Bokeh Mode aktivieren
        dof.mode.value = DepthOfFieldMode.Bokeh;
        dof.focusDistance.value = maxFocusDistance;

        currentFocusDistance = maxFocusDistance;
        targetFocusDistance = maxFocusDistance;


    }

    void Update()
    {
        UpdateFocusTarget();
        SmoothFocus();
        UpdateDOFValues();
    }

    void UpdateFocusTarget()
    {
        Vector3 origin = playerCamera.transform.position;
        Vector3 forward = Quaternion.Euler(rayDownwardAngle, 0f, 0f) * playerCamera.transform.forward;
        float maxDist = maxFocusDistance;

        // 1️⃣ Primärer Raycast
        Ray ray = new Ray(origin, forward);
        //RenderRaycast(ray.origin, ray.direction, maxDist);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDist, focusLayer))
        {
            targetFocusDistance = hit.distance;
            return;
        }

        // 2️⃣ Cone-Toleranz: Backup Fokus
        Collider[] candidates = Physics.OverlapSphere(origin + forward * maxDist * 0.5f, maxDist * 0.5f, focusLayer);
        float closestAngle = coneAngle;
        float closestDistance = maxFocusDistance;
        bool found = false;

        foreach (var col in candidates)
        {
            Vector3 toTarget = (col.bounds.center - origin).normalized;
            float angle = Vector3.Angle(forward, toTarget);
            if (angle < closestAngle)
            {
                float distance = Vector3.Distance(origin, col.bounds.center);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetFocusDistance = distance;
                    found = true;
                }
            }
        }

        if (!found)
            targetFocusDistance = maxFocusDistance;
    }

    void SmoothFocus()
    {
        currentFocusDistance = Mathf.SmoothDamp(currentFocusDistance, targetFocusDistance, ref focusVelocity, focusSmoothTime);
    }

    void UpdateDOFValues()
    {
        float aperture;

        if (currentFocusDistance <= nearSoftDistance)
        {
            float t = currentFocusDistance / nearSoftDistance; // 0 = nah, 1 = 2m
            aperture = Mathf.Lerp(maxAperture, minAperture, t * t);
        }
        else
        {
            aperture = minAperture;
        }

        float focusDist = (currentFocusDistance > nearSoftDistance) ? maxFocusDistance : currentFocusDistance;

        dof.focusDistance.value = focusDist;
        dof.aperture.value = aperture;
        dof.focalLength.value = focalLength;
        dof.bladeCount.value = bladeCount;
    }

    void RenderRaycast(Vector3 origin, Vector3 direction, float length)
    {
        if (rayVisualizer == null) return;

        rayVisualizer.SetPosition(0, origin);
        rayVisualizer.SetPosition(1, origin + direction * length);
    }

    void CreateRayVisual()
    {
        rayVisualizer = gameObject.AddComponent<LineRenderer>();
        rayVisualizer.positionCount = 2;
        rayVisualizer.startWidth = 0.02f;
        rayVisualizer.endWidth = 0.02f;
        rayVisualizer.material = new Material(Shader.Find("Sprites/Default"));
        rayVisualizer.startColor = Color.red;
        rayVisualizer.endColor = Color.red;
    }
}