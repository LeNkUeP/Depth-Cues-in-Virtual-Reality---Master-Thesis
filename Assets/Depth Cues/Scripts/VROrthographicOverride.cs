using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Camera))]
public class VROrthographicOverride : MonoBehaviour
{
    public float size = 10f; // „orthographic“ Größe
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void OnPreCull()
    {
        if (!XRSettings.isDeviceActive) return;

        // Nähe/Ferne Clipping
        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;

        // Orthographic-Matrix selbst bauen
        Matrix4x4 ortho = Matrix4x4.Ortho(
            -size, size,   // links/rechts
            -size, size,   // unten/oben
            near, far      // near/far
        );

        // Pro Auge anwenden
        cam.projectionMatrix = ortho;
    }
}

