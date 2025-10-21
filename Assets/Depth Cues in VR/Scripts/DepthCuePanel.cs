using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class DepthCuePanel : MonoBehaviour
{
    private static bool shadowCastEnabled = true;
    private static bool shapeFromShadingEnabled = true;
    private static bool occlusionEnabled = true;
    private static bool disparityEnabled = true;
    private static bool motionParallaxEnabled = true;
    private static bool atmosphericPerspectiveEnabled = true;

    private static Vector3 startPosition;
    private static Camera cam;
    private static Vector3 cameraLocalScale;
    private static Renderer[] allRenderers;
    private static Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private static Dictionary<Material, Material> unlitCache = new Dictionary<Material, Material>();

    public void Start()
    {
        allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        cam = Camera.main;
    }

    public void ToggleAtmosphericPerspective()
    {
        atmosphericPerspectiveEnabled = !atmosphericPerspectiveEnabled;

        if (!atmosphericPerspectiveEnabled)
        {
            RenderSettings.fog = false;
        }
        else
        {
            RenderSettings.fog = true;
        }
    }

    public static void ToggleMotionParallax()
    {
        motionParallaxEnabled = !motionParallaxEnabled;

        CharacterController characterController = FindFirstObjectByType<CharacterController>();
        TrackedPoseDriver poseDriver = cam.transform.GetComponent<TrackedPoseDriver>();
        startPosition = cam.transform.position;

        if (!motionParallaxEnabled)
        {
            poseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
            cam.transform.position = startPosition;
            characterController.enabled = false;
        }else
        {
            poseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
            characterController.enabled = true;
        }

        Debug.Log("Motion Parallax: " + (motionParallaxEnabled ? "ENABLED" : "DISABLED"));
    }

    public static void ToggleDisparity()
    {
        disparityEnabled = !disparityEnabled;

        if (!disparityEnabled)
        {
            cameraLocalScale = cam.transform.localScale;
            cam.transform.localScale = Vector3.zero;
        }
        else
        {
            cam.transform.localScale = cameraLocalScale;
        }

        Debug.Log("Disparity: " + (disparityEnabled ? "ENABLED" : "DISABLED"));
    }

    //void OnPreCull()
    //{
    //    if (disparityEnabled && cam != null && XRSettings.enabled)
    //    {
    //        Matrix4x4 leftView = cam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
    //        Matrix4x4 leftProj = cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);

    //        cam.SetStereoViewMatrix(Camera.StereoscopicEye.Right, leftView);
    //        cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, leftProj);
    //    }
    //    else if (!disparityEnabled && cam != null)
    //    {
    //        cam.ResetStereoViewMatrices();
    //        cam.ResetStereoProjectionMatrices();
    //    }
    //}

    public static void ToggleShadowCast()
    {
        shadowCastEnabled = !shadowCastEnabled;

        foreach (Renderer rend in allRenderers)
        {
            rend.shadowCastingMode = shadowCastEnabled ? ShadowCastingMode.On : ShadowCastingMode.Off;
            rend.receiveShadows = shadowCastEnabled;
        }

        Debug.Log("Shadow Cast " + (shadowCastEnabled ? "enabled" : "disabled"));
    }

    public static void ToggleShapeFromShading()
    {
        shapeFromShadingEnabled = !shapeFromShadingEnabled;

        foreach (Renderer rend in allRenderers)
        {
            if (rend.gameObject.CompareTag("Ground"))
                continue;

            if (shapeFromShadingEnabled)
            {
                // Ursprüngliche Materialien zurücksetzen
                if (originalMaterials.ContainsKey(rend))
                    rend.materials = originalMaterials[rend];
            }
            else
            {
                // Originalmaterialien merken
                if (!originalMaterials.ContainsKey(rend))
                    originalMaterials[rend] = rend.materials;

                // Ersetze jedes Material durch Unlit-Version
                Material[] mats = new Material[rend.materials.Length];
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    Material original = rend.materials[i];

                    if (unlitCache.ContainsKey(original))
                    {
                        mats[i] = unlitCache[original];
                    }
                    else
                    {
                        Material unlit = new Material(Shader.Find("Custom/UnlitWithShadowReceiver"));

                        if (original.HasProperty("_MainTex") && original.mainTexture != null)
                            unlit.mainTexture = original.mainTexture;
                        if (original.HasProperty("_Color"))
                            unlit.color = original.color;

                        mats[i] = unlit;
                        unlitCache[original] = unlit;
                    }
                }
                rend.materials = mats;
            }
        }

        Debug.Log("Shape From Shading " + (shapeFromShadingEnabled ? "enabled (Lit)" : "disabled (Unlit)"));
    }

    public static void ToggleOcclusion()
    {
        if (true)
        {
            return;
        }

        occlusionEnabled = !occlusionEnabled;

        foreach (Renderer rend in allRenderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (occlusionEnabled)
                {
                    // Depth Test und Write wieder normal
                    mat.SetInt("_ZTest", (int)CompareFunction.LessEqual);
                    mat.SetInt("_ZWrite", 1);
                    //mat.renderQueue = -1;
                }
                else
                {
                    // Depth Test auf Always und ZWrite aus
                    mat.SetInt("_ZTest", (int)CompareFunction.Greater);
                    mat.SetInt("_ZWrite", 0);
                    //mat.renderQueue = (int)RenderQueue.Transparent;
                    //mat.renderQueue = 5000;
                }
            }
        }

        Debug.Log("Occlusion " + (occlusionEnabled ? "enabled" : "disabled"));
    }

    //public void ToggleOcclusion()
    //{
    //    StartCoroutine(ChangeRenderQueueLoop());
    //    Debug.Log("Occlusion " + (occlusionEnabled ? "enabled" : "disabled"));
    //}

    //public static int baseQueue = 3000;

    //public static IEnumerator ChangeRenderQueueLoop()
    //{
    //    List<Renderer> renderers = allRenderers.ToList();
    //    occlusionEnabled = !occlusionEnabled;

    //    while (true)
    //    {
    //        if (renderers == null || renderers.Count == 0)
    //            yield break;

    //        // Rotiert die Liste
    //        Renderer last = renderers[renderers.Count - 1];
    //        renderers.RemoveAt(renderers.Count - 1);
    //        renderers.Insert(0, last);

    //        // Neue RenderQueue-Werte setzen
    //        for (int i = 0; i < renderers.Count; i++)
    //        {
    //            if (renderers[i] != null)
    //            {
    //                Material mat = renderers[i].material;
    //                mat.renderQueue = baseQueue + i;
    //            }
    //        }

    //        yield return new WaitForSeconds(0f);
    //    }
    //}
}
